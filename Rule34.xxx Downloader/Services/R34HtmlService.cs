using HtmlAgilityPack;
using R34Downloader.Models;
using System;
using System.IO;
using System.Linq;

namespace R34Downloader.Services
{
    /// <summary>
    /// HTML parsing service.
    /// </summary>
    public static class R34HtmlService
    {
        #region Fields

        private const string ContentUrl = "https://rule34.xxx/index.php?page=post&s=list&tags=";

        private const byte PageSize = 42;

        #endregion

        #region Methods

        /// <summary>
        /// Checks for the presence of content for the specified tags.
        /// </summary>
        /// <param name="tags">Tags.</param>
        /// <returns>Return True if any content is found, otherwise False.</returns>
        public static bool IsSomethingFound(string tags)
        {
            var document = LoadHtmlDocument($"{ContentUrl}{tags}");
            var nodes = document.DocumentNode.SelectNodes("//div[@class='content']//span[@class='thumb']");

            return nodes != null;
        }

        /// <summary>
        /// Returns the maximum page for the specified tags.
        /// </summary>
        /// <param name="tags">Tags.</param>
        /// <returns>Returns the maximum page or 0 if nothing is found.</returns>
        public static int GetMaxPid(string tags)
        {
            var document = LoadHtmlDocument($"{ContentUrl}{tags}");
            var nodes = document.DocumentNode.SelectSingleNode("//div[@class='pagination']//a[@alt='last page']");
            var pidString = nodes?.GetAttributeValue("href", null);

            if (pidString == null)
            {
                return default;
            }

            var maxPid = pidString.Substring(pidString.LastIndexOf('=') + 1, pidString.Length - pidString.LastIndexOf('=') - 1);

            return Convert.ToInt32(maxPid);
        }

        /// <summary>
        /// Returns the amount of content on the specified page for the specified tags.
        /// </summary>
        /// <param name="tags">Tags.</param>
        /// <param name="pid">Page.</param>
        /// <returns>Returns the amount of content on the page, or -1 if nothing is found.</returns>
        public static int GetCountContent(string tags, int pid)
        {
            var document = LoadHtmlDocument($"{ContentUrl}{tags}&pid={pid}");
            var nodes = document.DocumentNode.SelectNodes("//div[@class='content']//span[@class='thumb']/a");

            if (nodes != null && pid == 0)
            {
                return nodes.Count;
            }

            if (nodes != null && pid != 0)
            {
                return pid + nodes.Count;
            }

            return -1;
        }

        /// <summary>
        /// Downloads the specified content in the specified quantity.
        /// </summary>
        /// <param name="path">Path to save files.</param>
        /// <param name="tags">Tags.</param> 
        /// <param name="quantity">Quantity.</param>
        /// <param name="progress"><see cref="IProgress{T}"/></param>
        /// <param name="progress2"><see cref="IProgress{T}"/></param>
        public static void DownloadContent(string path, string tags, ushort quantity, IProgress<int> progress, IProgress<int> progress2)
        {
            var maxPages = quantity;
            ushort residue = PageSize;

            if (quantity < PageSize)
            {
                maxPages = PageSize;
                residue = quantity;
            }

            for (var pid = 0; pid < maxPages; pid += PageSize)
            {
                var document = LoadHtmlDocument($"{ContentUrl}{tags}&pid={pid}");
                var nodes = document.DocumentNode.SelectNodes("//div[@class='content']//span[@class='thumb']/a");

                var posts = nodes.Select(x => x.GetAttributeValue("href", "").Replace("&amp;", "&"))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();

                DownloadPosts(posts, path, pid, residue, maxPages, progress, progress2);
            }
        }

        #endregion

        #region Helpers

        private static void DownloadPosts(string[] posts, string path, int pid, int residue, int maxPages, IProgress<int> progress, IProgress<int> progress2)
        {
            var maxPosts = posts.Length;
            if (maxPages - pid < PageSize)
            {
                maxPosts = maxPages - pid;
            }
            else if (maxPages - pid == PageSize)
            {
                maxPosts = residue;
            }

            for (var i = 0; i < maxPosts; i++)
            {
                var document = LoadHtmlDocument($"https://rule34.xxx/{posts[i]}");

                var videoNode = document.DocumentNode.SelectSingleNode("//video[@id='gelcomVideoPlayer']/source");
                var imageNode = document.DocumentNode.SelectSingleNode("//div[@class='content']//img[@id='image']");

                if (videoNode != null && SettingsModel.Video)
                {
                    var videoUrl = videoNode.GetAttributeValue("src", null);
                    if (videoUrl != null)
                    {
                        var filename = Path.GetFileName(videoUrl);
                        var questionMarkIndex = filename.IndexOf('?');
                        if (questionMarkIndex > 0)
                        {
                            filename = Path.GetFileName(filename.Substring(0, questionMarkIndex));
                        }

                        DownloadService.Download(videoUrl, Path.Combine(path, "Video", filename));
                    }
                }
                else
                {
                    var imageUrl = imageNode?.GetAttributeValue("src", null);
                    if (imageUrl != null)
                    {
                        var id = imageUrl.Split('?')[1];
                        imageUrl = imageUrl.Substring(0, imageUrl.LastIndexOf('?'));
                        var filename = $"{id}{Path.GetExtension(imageUrl)}";

                        if (filename.Contains(".gif") && SettingsModel.Gif)
                        {
                            DownloadService.Download(imageUrl, Path.Combine(path, "Gif", filename));
                        }
                        else if (!filename.Contains(".gif") && SettingsModel.Images)
                        {
                            DownloadService.Download(imageUrl, Path.Combine(path, "Images", filename));
                        }
                    }
                }

                var reportStatus = pid + i + 1;
                progress.Report(reportStatus);
                progress2.Report(reportStatus);
            }
        }

        private static HtmlDocument LoadHtmlDocument(string url)
        {
            return new HtmlWeb().Load(url);
        }

        #endregion
    }
}
