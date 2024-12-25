using R34.Downloader.Domain.Models.Api;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace R34.Downloader.Application.Services
{
    public class DownloadService
    {
        private readonly HttpClient _httpClient;

        private const string DateTimeFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

        /// <summary>
        /// Initializes a new instance of the DownloadService class.
        /// </summary>
        public DownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> TryDownloadAsync(PostModel post, string saveDirectory, bool downloadInMaxQuality, CancellationToken cancellationToken)
        {
            var downloadUrl = !downloadInMaxQuality && !string.IsNullOrEmpty(post.SampleUrl) ? post.SampleUrl : post.FileUrl;
            var filePath = Path.Combine(saveDirectory, $"{post.Id}{Path.GetExtension(post.FileUrl)}");
            if (File.Exists(filePath))
            {
                return false;
            }

            try
            {
                using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                await using var stream = await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync(cancellationToken);
                await using var fileStream = File.Create(filePath);
                await stream.CopyToAsync(fileStream, cancellationToken);

                try
                {
                    File.SetCreationTime(filePath, DateTimeOffset.ParseExact(post.CreatedAt, DateTimeFormat, CultureInfo.InvariantCulture).ToLocalTime().DateTime);
                }
                catch (Exception)
                {
                    // Ignored
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
