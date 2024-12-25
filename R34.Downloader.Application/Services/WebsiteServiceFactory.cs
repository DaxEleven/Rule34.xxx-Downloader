using R34.Downloader.Domain.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace R34.Downloader.Application.Services
{
    public static class WebsiteServiceFactory
    {
        public static async Task<WebsiteService> GetWebsiteServiceAsync(HttpClient httpClient, XmlSerializer xmlSerializer)
        {
            WebsiteService websiteService = new WebsiteApiService(httpClient, xmlSerializer);
            if (await IsWebsiteServiceWorkingAsync(websiteService))
            {
                return websiteService;
            }

            websiteService = new WebsiteHtmlService(httpClient);
            if (await IsWebsiteServiceWorkingAsync(websiteService))
            {
                return websiteService;
            }

            static async Task<bool> IsWebsiteServiceWorkingAsync(WebsiteService service)
            {
                try
                {
                    await service.GetContentCountAsync(new SearchPhrase());
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }

            return null;
        }
    }
}
