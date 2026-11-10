using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace R34Downloader.Services
{
    /// <summary>
    /// Download service. Files are streamed to disk, so large videos do not
    /// have to fit into memory, and the current file progress can be reported.
    /// </summary>
    public static class DownloadService
    {
        #region Fields

        private const int BufferSize = 81920;
        private const int ProgressIntervalMilliseconds = 150;

        /// <summary>
        /// Suffix of the file that is being written. The file is renamed only when the
        /// transfer is complete, so an interrupted download never looks finished.
        /// </summary>
        internal const string TemporaryFileSuffix = ".part";

        private static readonly HttpClient Client = CreateClient();

        #endregion

        #region Properties

        /// <summary>
        /// Reports the progress of the current file: downloaded bytes and total bytes
        /// (the total is null when the server does not send Content-Length). May be null.
        /// </summary>
        public static Action<long, long?> OnFileProgress { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Downloads and saves a file at the specified path.
        /// </summary>
        /// <param name="url">File url.</param>
        /// <param name="filePath">File path with name.</param>
        /// <returns>True if the file was downloaded now, False if it already exists.</returns>
        public static bool Download(string url, string filePath)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new R34Exception("Empty file url.");
            }

            // The name of a file contains the post id, so the check is done by name.
            // The cache enumerates every download folder only once per run, which makes
            // resuming a large download instant (no file system request per post).
            if (DownloadedFilesCache.Contains(filePath))
            {
                return false;
            }

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var temporaryPath = filePath + TemporaryFileSuffix;
            Exception lastError = null;

            for (var attempt = 1; attempt <= RetryPolicy.MaxAttempts; attempt++)
            {
                try
                {
                    TryDelete(temporaryPath);

                    using (var response = Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                    {
                        if (RetryPolicy.IsTransient(response.StatusCode))
                        {
                            var delay = RetryPolicy.GetDelay(response, attempt);
                            RetryPolicy.Report(
                                $"Temporary media error (HTTP {(int)response.StatusCode}), retry {attempt + 1}/{RetryPolicy.MaxAttempts} in {delay.TotalSeconds:0} s...");

                            if (attempt < RetryPolicy.MaxAttempts)
                            {
                                Thread.Sleep(delay);

                                continue;
                            }

                            throw new R34Exception($"The server is unavailable (HTTP {(int)response.StatusCode}).");
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new R34Exception($"The server returned HTTP {(int)response.StatusCode} ({response.ReasonPhrase}).");
                        }

                        CopyToFile(response, temporaryPath);
                    }

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    File.Move(temporaryPath, filePath);
                    DownloadedFilesCache.Register(filePath);

                    return true;
                }
                catch (Exception exception) when (attempt < RetryPolicy.MaxAttempts && RetryPolicy.IsTransient(exception))
                {
                    lastError = exception;

                    var delay = RetryPolicy.GetBackoff(attempt);
                    RetryPolicy.Report($"Network problem, retry {attempt + 1}/{RetryPolicy.MaxAttempts} in {delay.TotalSeconds:0} s...");
                    Thread.Sleep(delay);
                }
                catch (Exception exception)
                {
                    lastError = exception;

                    break;
                }
            }

            TryDelete(temporaryPath);

            throw new R34Exception($"Failed to download {Path.GetFileName(filePath)}: {lastError?.Message}", lastError);
        }

        #endregion

        #region Helpers

        private static void CopyToFile(HttpResponseMessage response, string filePath)
        {
            var totalBytes = response.Content.Headers.ContentLength;

            using (var source = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
            using (var target = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, FileOptions.SequentialScan))
            {
                var buffer = new byte[BufferSize];
                long downloadedBytes = 0;
                var progressWatch = Stopwatch.StartNew();

                ReportProgress(0, totalBytes);

                while (true)
                {
                    var read = source.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        break;
                    }

                    target.Write(buffer, 0, read);
                    downloadedBytes += read;

                    if (progressWatch.ElapsedMilliseconds >= ProgressIntervalMilliseconds)
                    {
                        progressWatch.Restart();
                        ReportProgress(downloadedBytes, totalBytes);
                    }
                }

                ReportProgress(downloadedBytes, totalBytes);

                if (downloadedBytes == 0)
                {
                    throw new R34Exception("The server returned an empty file.");
                }

                if (totalBytes.HasValue && downloadedBytes != totalBytes.Value)
                {
                    throw new IOException("The response ended before the complete file was received.");
                }
            }
        }

        private static void ReportProgress(long downloadedBytes, long? totalBytes)
        {
            try
            {
                OnFileProgress?.Invoke(downloadedBytes, totalBytes);
            }
            catch
            {
                // A status update must never break the download.
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static HttpClient CreateClient()
        {
            // A large video may take a while; the retry policy handles real failures.
            return HttpClientHelper.CreateClient(HttpClientHelper.MediaAccept, TimeSpan.FromMinutes(30), true);
        }

        #endregion
    }
}
