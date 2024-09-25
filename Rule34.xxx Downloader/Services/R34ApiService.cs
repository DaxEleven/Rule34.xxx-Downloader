using Newtonsoft.Json;
using R34Downloader.Models;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="useAria2">Use aria2 for downloading.</param>
        /// <param name="aria2ServerUrl">Aria2 server URL.</param>
        public static void DownloadContent(string path, string tags, ushort quantity, IProgress<int> progress, IProgress<int> progress2, bool useAria2 = false)
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
                        var filePath = string.Empty;
                        if ((url.Contains(".mp4") || url.Contains(".webm")) && SettingsModel.Video)
                        {
                            filePath = Path.Combine(path, "Video", filename);
                        }
                        else if (url.Contains(".gif") && SettingsModel.Gif)
                        {
                            filePath = Path.Combine(path, "Gif", filename);
                        }
                        else if (!url.Contains(".mp4") && !url.Contains(".webm") && !url.Contains(".gif") && SettingsModel.Images)
                        {
                            filePath = Path.Combine(path, "Images", filename);
                        }

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            if (useAria2 && !string.IsNullOrEmpty(SettingsModel.Aria2ServerUrl))
                            {
                                DownloadWithAria2(url, filePath, SettingsModel.Aria2ServerUrl, SettingsModel.Aria2SecretToken).Wait();
                            }
                            else
                            {
                                DownloadService.Download(url, filePath);
                            }
                        }
                    }

                    var reportStatus = pid * 100 + i + 1;
                    progress.Report(reportStatus);
                    progress2.Report(reportStatus);
                }
            }
        }

        private static async Task DownloadWithAria2(string url, string filePath, string aria2ServerUrl, string aria2SecretToken)
        {
            using (var client = new HttpClient())
            {
                var jsonPayload = new
                {
                    jsonrpc = "2.0",
                    method = "aria2.addUri",
                    id = "1",
                    @params = new object[]
                    {
                            new string[] { url },
                            new
                            {
                                @out = Path.GetFileName(filePath)
                            }
                    }
                };

                if (!string.IsNullOrEmpty(aria2SecretToken))
                {
                    jsonPayload = new
                    {
                        jsonrpc = "2.0",
                        method = "aria2.addUri",
                        id = "1",
                        @params = new object[]
                        {
                                "token:" + aria2SecretToken,
                                new string[] { url },
                                new
                                {
                                    @out = Path.GetFileName(filePath)
                                }
                        }
                    };
                }

                var content = new StringContent(JsonConvert.SerializeObject(jsonPayload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(aria2ServerUrl, content);
                response.EnsureSuccessStatusCode();
            }
        }

        #endregion
    }
}
