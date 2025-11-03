using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace R34Downloader.Services
{
    /// <summary>
    /// Download service.
    /// </summary>
    public static class DownloadService
    {
        #region Methods

        /// <summary>
        /// Downloads and saves a file at the specified path.
        /// </summary>
        /// <param name="url">File url.</param>
        /// <param name="filePath">File path with name.</param>
        public static void Download(string url, string filePath)
        {
            if (!File.Exists(filePath))
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new CookieContainer()
                };

                handler.CookieContainer.Add(new Cookie
                {
                    Name = "gdpr",
                    Value = "1",
                    Domain = "rule34.xxx",
                    Path = "/",
                    Expires = DateTime.Now.AddYears(1)
                });

                handler.CookieContainer.Add(new Cookie
                {
                    Name = "gdpr-consent",
                    Value = "1",
                    Domain = "rule34.xxx",
                    Path = "/",
                    Expires = DateTime.Now.AddYears(1)
                });

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.Referrer = new Uri("https://rule34.xxx/");

                    try
                    {
                        var response = client.GetAsync(url).Result;
                        response.EnsureSuccessStatusCode();

                        var data = response.Content.ReadAsByteArrayAsync().Result;
                        File.WriteAllBytes(filePath, data);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        #endregion
    }
}
