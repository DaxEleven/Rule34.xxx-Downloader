using R34.Downloader.Domain.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace R34.Downloader.Application.Services
{
    public class WebsiteHtmlService : WebsiteService
    {
        public WebsiteHtmlService(HttpClient httpClient) : base(httpClient)
        {
        }

        public override Task<uint> GetContentCountAsync(SearchPhrase searchPhrase, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task DownloadContentAsync(Settings settings, IProgress<DownloadStatus> progress, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
