using System.IO;
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
                var directory = filePath.Substring(0, filePath.LastIndexOf(Path.DirectorySeparatorChar));
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var client = new HttpClient())
                {
                    try
                    {
                        client.DefaultRequestHeaders.Referrer = new Uri("https://rule34.xxx/");
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36");
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
