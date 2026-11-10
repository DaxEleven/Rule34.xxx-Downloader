using R34Downloader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace R34Downloader.Services
{
    /// <summary>
    /// API parsing service.
    /// </summary>
    /// <remarks>
    /// The official API endpoint is https://api.rule34.xxx and it requires the credentials of the current user:
    /// <c>user_id</c> (account id) and <c>api_key</c> (key from https://rule34.xxx/index.php?page=account&amp;s=options).
    /// The old endpoint (rule34.xxx/index.php?page=dapi) is no longer served to non-browser clients.
    /// </remarks>
    public static class R34ApiService
    {
        #region Fields

        /// <summary>
        /// Page where every user can generate his own API key.
        /// </summary>
        public const string ApiKeyPageUrl = "https://rule34.xxx/index.php?page=account&s=options";

        private const string ApiUrl = "https://api.rule34.xxx/index.php";

        private const int PageSize = 100;

        private const int MaxPages = 1000;

        private static readonly HttpClient Client = CreateClient();

        #endregion

        #region Methods

        /// <summary>
        /// Returns the amount of content for the given tags using the credentials from the settings.
        /// </summary>
        /// <param name="tags">Tags.</param>
        /// <returns>Content count.</returns>
        public static int GetContentCount(string tags)
        {
            return GetContentCount(tags, SettingsModel.UserId, SettingsModel.ApiKey);
        }

        /// <summary>
        /// Returns the amount of content for the given tags. Used to check the entered credentials.
        /// </summary>
        /// <param name="tags">Tags.</param>
        /// <param name="userId">Account id.</param>
        /// <param name="apiKey">API key.</param>
        /// <returns>Content count.</returns>
        public static int GetContentCount(string tags, string userId, string apiKey)
        {
            var root = GetPostsElement(tags, 0, 1, userId, apiKey);
            var count = root.Attribute("count")?.Value;

            if (int.TryParse(count, out var result))
            {
                return result;
            }

            return root.Elements("post").Any() ? SettingsModel.MaxLimit : 0;
        }

        /// <summary>
        /// Returns one page of posts.
        /// </summary>
        /// <param name="tags">Tags.</param>
        /// <param name="pid">Page number (zero based).</param>
        /// <param name="limit">Amount of posts on the page.</param>
        /// <returns>Posts.</returns>
        public static List<PostModel> GetPosts(string tags, int pid, int limit)
        {
            var root = GetPostsElement(tags, pid, limit, SettingsModel.UserId, SettingsModel.ApiKey);

            return ParsePosts(root);
        }

        /// <summary>
        /// Downloads the specified content in the specified quantity.
        /// </summary>
        /// <param name="path">Path to save files.</param>
        /// <param name="tags">Tags.</param>
        /// <param name="quantity">Quantity of files to download.</param>
        /// <param name="progress"><see cref="IProgress{T}"/></param>
        /// <param name="progress2"><see cref="IProgress{T}"/></param>
        /// <returns>Amount of files that could not be downloaded.</returns>
        public static int DownloadContent(string path, string tags, int quantity, IProgress<int> progress, IProgress<int> progress2)
        {
            var downloaded = 0;
            var failed = 0;

            // Files that are already on disk are taken from the cache, which is built once per run for every download folder
            DownloadedFilesCache.Reset();

            // Posts of a disabled type are skipped, so more pages may be needed than quantity / PageSize; allow up to four times as many
            var maxPages = Math.Max(MaxPages, quantity / PageSize * 4 + 10);

            for (var pid = 0; downloaded < quantity && pid < maxPages; pid++)
            {
                if (DownloadControlService.StopRequested)
                {
                    break;
                }

                var posts = GetPosts(tags, pid, PageSize);

                if (posts.Count == 0)
                {
                    break;
                }

                foreach (var post in posts)
                {
                    if (DownloadControlService.StopRequested || downloaded >= quantity)
                    {
                        break;
                    }

                    if (!IsWanted(post))
                    {
                        continue;
                    }

                    try
                    {
                        var downloadedNow = DownloadPost(path, post);

                        downloaded++;
                        progress.Report(downloaded);
                        progress2.Report(downloaded);

                        if (downloadedNow)
                        {
                            HttpClientHelper.Pause();
                        }
                    }
                    catch (Exception exception) when (exception is R34Exception || exception is IOException || exception is UnauthorizedAccessException)
                    {
                        failed++;
                        HttpClientHelper.Pause();
                    }
                }

                if (downloaded < quantity)
                {
                    HttpClientHelper.Pause();
                }
            }

            return failed;
        }

        #endregion

        #region Helpers

        private static bool IsWanted(PostModel post)
        {
            if (post.IsVideo)
            {
                return SettingsModel.Video;
            }

            if (post.IsGif)
            {
                return SettingsModel.Gif;
            }

            return SettingsModel.Images;
        }

        private static bool DownloadPost(string path, PostModel post)
        {
            var url = post.DownloadUrl;
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new R34Exception($"Post {post.Id} has no file url.");
            }

            var folder = post.IsVideo ? "Video" : post.IsGif ? "Gif" : "Images";
            var fileName = post.Id + post.Extension;

            return DownloadService.Download(url, Path.Combine(path, folder, fileName));
        }

        private static XElement GetPostsElement(string tags, int pid, int limit, string userId, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(apiKey))
            {
                throw new R34Exception("User ID and/or API key are not specified.\r\n\r\n" +
                                       "Open the settings and enter the credentials of your own rule34.xxx account.\r\n" +
                                       "The key is generated here: " + ApiKeyPageUrl + "\r\n\r\n" +
                                       "The API key is personal: every user has his own user_id and api_key.");
            }

            var url = new StringBuilder(ApiUrl)
                .Append("?page=dapi&s=post&q=index")
                .Append("&limit=").Append(limit)
                .Append("&pid=").Append(pid)
                .Append("&tags=").Append(Uri.EscapeDataString(NormalizeTags(tags)))
                .Append("&user_id=").Append(Uri.EscapeDataString(userId.Trim()))
                .Append("&api_key=").Append(Uri.EscapeDataString(apiKey.Trim()))
                .ToString();

            return Request(url);
        }

        private static XElement Request(string url)
        {
            Exception lastError = null;

            for (var attempt = 1; attempt <= RetryPolicy.MaxAttempts; attempt++)
            {
                try
                {
                    using (var response = Client.GetAsync(url).GetAwaiter().GetResult())
                    {
                        if (RetryPolicy.IsTransient(response.StatusCode))
                        {
                            lastError = new R34Exception($"The API is temporarily unavailable (HTTP {(int)response.StatusCode}).");

                            if (attempt < RetryPolicy.MaxAttempts)
                            {
                                var delay = RetryPolicy.GetDelay(response, attempt);
                                RetryPolicy.Report($"API is busy (HTTP {(int)response.StatusCode}), retry {attempt + 1}/{RetryPolicy.MaxAttempts} in {delay.TotalSeconds:0} s...");
                                Thread.Sleep(delay);

                                continue;
                            }

                            throw lastError;
                        }

                        using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                        {
                            XDocument document;

                            try
                            {
                                document = XDocument.Load(stream);
                            }
                            catch (XmlException)
                            {
                                throw new R34Exception($"The API returned a response that could not be parsed (HTTP {(int)response.StatusCode}). " +
                                                       "The request may be blocked by Cloudflare or the response format has changed.");
                            }

                            if (document.Root == null)
                            {
                                throw new R34Exception("The API returned an empty response.");
                            }

                            // A broken searcher is reported as success="false" in the root element.
                            if (string.Equals((string)document.Root.Attribute("success"), "false", StringComparison.OrdinalIgnoreCase))
                            {
                                var reason = document.Root.Value.Trim();

                                throw new R34Exception("The rule34.xxx search is down right now." + (reason.Length > 0 ? " " + reason : string.Empty));
                            }

                            if (!string.Equals(document.Root.Name.LocalName, "posts", StringComparison.OrdinalIgnoreCase))
                            {
                                throw new R34Exception(BuildApiErrorMessage(document.Root.Value, response.StatusCode));
                            }

                            return document.Root;
                        }
                    }
                }
                catch (R34Exception)
                {
                    throw;
                }
                catch (Exception exception) when (attempt < RetryPolicy.MaxAttempts && RetryPolicy.IsTransient(exception))
                {
                    lastError = exception;

                    var delay = RetryPolicy.GetBackoff(attempt);
                    RetryPolicy.Report($"Network problem, retry {attempt + 1}/{RetryPolicy.MaxAttempts} in {delay.TotalSeconds:0} s...");
                    Thread.Sleep(delay);
                }
                catch (HttpRequestException exception)
                {
                    throw new R34Exception("Could not connect to the rule34.xxx API. Check your internet connection.", exception);
                }
                catch (TaskCanceledException exception)
                {
                    throw new R34Exception("The rule34.xxx API request timed out.", exception);
                }
            }

            throw new R34Exception("Could not get a response from the rule34.xxx API.", lastError);
        }

        private static string BuildApiErrorMessage(string serverMessage, HttpStatusCode statusCode)
        {
            var message = string.IsNullOrWhiteSpace(serverMessage) ? "empty response" : serverMessage.Trim();
            if (message.IndexOf("authentication", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "The API key was not accepted (" + message + ").\r\n\r\n" +
                       "Check the User ID and API key in the settings: both must belong to the same account.\r\n" +
                       "Get your key here: " + ApiKeyPageUrl + "\r\n\r\n" +
                       "Do not create several keys: the site suspends accounts for that.";
            }

            return $"rule34.xxx API error (HTTP {(int)statusCode}): {message}";
        }

        private static string NormalizeTags(string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                return string.Empty;
            }

            var parts = tags.Split(new[] { ' ', '+' }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(" ", parts);
        }

        private static List<PostModel> ParsePosts(XElement root)
        {
            var posts = new List<PostModel>();
            foreach (var element in root.Elements("post"))
            {
                posts.Add(new PostModel
                {
                    Id = (string)element.Attribute("id"),
                    FileUrl = (string)element.Attribute("file_url"),
                    SampleUrl = (string)element.Attribute("sample_url"),
                    PreviewUrl = (string)element.Attribute("preview_url")
                });
            }

            return posts;
        }

        private static HttpClient CreateClient()
        {
            return HttpClientHelper.CreateClient(HttpClientHelper.ApiAccept, TimeSpan.FromSeconds(60), false);
        }

        #endregion
    }
}
