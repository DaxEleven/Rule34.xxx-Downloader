using R34.Downloader.Domain.Models;
using R34.Downloader.Domain.Models.Api;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace R34.Downloader.Application.Services
{
    /// <summary>
    /// Service for working with the Rule34 website.
    /// </summary>
    public abstract class WebsiteService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the WebsiteService class.
        /// </summary>
        protected WebsiteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Returns the amount of content found for a search phrase.
        /// </summary>
        /// <param name="searchPhrase"><see cref="SearchPhrase"/></param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns></returns>
        public abstract Task<uint> GetContentCountAsync(SearchPhrase searchPhrase, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task DownloadContentAsync(Settings settings, IProgress<DownloadStatus> progress, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchPhrase"><see cref="SearchPhrase"/></param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns></returns>
        public async Task<TagAutocompleteModel[]> GetAutocompleteResultAsync(SearchPhrase searchPhrase, CancellationToken cancellationToken = default)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://rule34.xxx/public/autocomplete.php?q={searchPhrase}");

            requestMessage.Headers.Referrer = new Uri("https://rule34.xxx/");

            using var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            return JsonSerializer.Deserialize<TagAutocompleteModel[]>(responseStream);
        }
    }
}
