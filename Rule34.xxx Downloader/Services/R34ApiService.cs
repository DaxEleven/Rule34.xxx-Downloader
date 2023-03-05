using R34Downloader.Models;
using System;
using System.IO;
using System.Xml;

namespace R34Downloader.Services
{
    /// <summary>
    /// API parsing service.
    /// </summary>
    public static class R34ApiService
    {
        #region Fields

        private const string ApiUrl = "https://rule34.xxx/index.php?page=dapi&s=post&q=index";

        private const byte PageSize = 100;

        #endregion

        #region Methods

        /// <summary>
        /// Returns the amount of content for the given tags.
        /// </summary>
        /// <param name="tags">Tags.</param>
        /// <returns>Content count.</returns>
        public static int GetContentCount(string tags)
        {
            var document = new XmlDocument();
            document.Load($"{ApiUrl}&tags={tags}");

            return int.TryParse(document.DocumentElement?.Attributes[0].Value, out var count) ? count : default;
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
            var maxPid = quantity <= PageSize ? 1 : quantity % PageSize == 0 ? quantity / PageSize - 1 : quantity / PageSize;

            for (var pid = 0; pid <= maxPid; pid++)
            {
                var doc = new XmlDocument();
                doc.Load($"{ApiUrl}&tags={tags}&pid={pid}");

                var postCount = quantity - pid * PageSize < PageSize ? quantity - pid * PageSize : PageSize;
                for (var i = 0; i < postCount; i++)
                {
                    var url = doc.DocumentElement?.ChildNodes[i].Attributes?[2].Value;
                    var filename = doc.DocumentElement?.ChildNodes[i].Attributes?[10].Value + url?.Substring(url.LastIndexOf('.'), url.Length - url.LastIndexOf('.'));

                    if (url != null)
                    {
                        if ((url.Contains(".mp4") || url.Contains(".webm")) && SettingsModel.Video)
                        {
                            DownloadService.Download(url, Path.Combine(path, "Video", filename));
                        }
                        else if (url.Contains(".gif") && SettingsModel.Gif)
                        {
                            DownloadService.Download(url, Path.Combine(path, "Gif", filename));
                        }
                        else if (!url.Contains(".mp4") && !url.Contains(".webm") && !url.Contains(".gif") && SettingsModel.Images)
                        {
                            DownloadService.Download(url, Path.Combine(path, "Images", filename));
                        }
                    }

                    var reportStatus = pid * 100 + i + 1;
                    progress.Report(reportStatus);
                    progress2.Report(reportStatus);
                }
            }
        }

        #endregion
    }
}
