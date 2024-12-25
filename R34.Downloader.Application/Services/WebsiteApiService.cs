using R34.Downloader.Domain.Enums;
using R34.Downloader.Domain.Models;
using R34.Downloader.Domain.Models.Api;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace R34.Downloader.Application.Services
{
    /// <inheritdoc cref="WebsiteService"/>
    public class WebsiteApiService : WebsiteService
    {
        #region Fields

        private readonly HttpClient _httpClient;

        private readonly DownloadService _downloadService;

        private readonly XmlSerializer _xmlSerializer;

        private const string Url = "https://rule34.xxx/index.php?page=dapi&s=post&q=index";

        private const byte PageSize = 100;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the WebsiteApiService class.
        /// </summary>
        /// <param name="httpClient"><see cref="HttpClient"/></param>
        /// <param name="xmlSerializer"><see cref="XmlSerializer"/></param>
        public WebsiteApiService(HttpClient httpClient, XmlSerializer xmlSerializer) : base(httpClient)
        {
            _httpClient = httpClient;
            _xmlSerializer = xmlSerializer;

            _downloadService = new DownloadService(_httpClient);
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override async Task<uint> GetContentCountAsync(SearchPhrase searchPhrase, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"{Url}&limit=0&tags={searchPhrase}", cancellationToken);
                await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var posts = (PostsModel)_xmlSerializer.Deserialize(responseStream);

                return posts?.Count ?? default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <inheritdoc />
        public override async Task DownloadContentAsync(Settings settings, IProgress<DownloadStatus> progress, CancellationToken cancellationToken = default)
        {
            var status = new DownloadStatus();
            string imagesFolder = null, videoFolder = null, gifFolder = null;

            var lastPage = settings.Quantity / PageSize;
            if (settings.Quantity % PageSize == 0)
            {
                lastPage--;
            }

            for (var page = 0; page <= lastPage; page++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var url = $"{Url}&tags={settings.SearchPhrase}&pid={page}";
                if (page == lastPage)
                {
                    url += $"&limit={settings.Quantity - lastPage * PageSize}";
                }

                try
                {
                    using var response = await _httpClient.GetAsync(url, cancellationToken);
                    await using var responseStream = await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync(cancellationToken);
                    var posts = (PostsModel)_xmlSerializer.Deserialize(responseStream);

                    if (posts?.Posts == null || posts.Posts.Length == 0)
                    {
                        return;
                    }

                    foreach (var post in posts.Posts)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        Task<bool> downloadTask;
                        switch (GetContentTypeFromUrl(post.FileUrl))
                        {
                            case ContentType.Image when settings.IncludeImages:
                            {
                                downloadTask = _downloadService.TryDownloadAsync(post,
                                    GetFolder(settings.SavePath, ref imagesFolder, "Images"),
                                    settings.DownloadContentInMaxQuality,
                                    cancellationToken);

                                break;
                            }
                            case ContentType.Video when settings.IncludeVideo:
                            {
                                downloadTask = _downloadService.TryDownloadAsync(post,
                                    GetFolder(settings.SavePath, ref videoFolder, "Video"),
                                    settings.DownloadContentInMaxQuality,
                                    cancellationToken);

                                break;
                            }
                            case ContentType.Gif when settings.IncludeGifs:
                            {
                                downloadTask = _downloadService.TryDownloadAsync(post,
                                    GetFolder(settings.SavePath, ref gifFolder, "Gif"),
                                    settings.DownloadContentInMaxQuality,
                                    cancellationToken);

                                break;
                            }
                            default:
                            {
                                downloadTask = null;

                                break;
                            }
                        }

                        if (downloadTask == null)
                        {
                            status.Skipped++;
                            progress.Report(status);

                            continue;
                        }

                        if (await downloadTask)
                        {
                            status.Downloaded++;
                        }
                        else
                        {
                            status.Skipped++;
                        }

                        progress.Report(status);
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("API CANCEL");
                    throw;
                }
                catch (Exception)
                {
                    Debug.WriteLine("API EXCEPTION");
                    return;
                }
            }
        }

        private static string GetFolder(string savePath, ref string folder, string name)
        {
            if (folder == null)
            {
                folder = Path.Combine(savePath, name);
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        private static ContentType GetContentTypeFromUrl(string url)
        {
            if (url.Contains(".mp4") || url.Contains(".webm"))
            {
                return ContentType.Video;
            }

            if (url.Contains(".gif"))
            {
                return ContentType.Gif;
            }

            return ContentType.Image;
        }
        

        #endregion
    }
}
