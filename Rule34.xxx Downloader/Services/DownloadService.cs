using System.IO;
using System.Net;

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

                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(url, filePath);
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
