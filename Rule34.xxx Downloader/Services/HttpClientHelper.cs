using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace R34Downloader.Services
{
    /// <summary>
    /// Creates HTTP clients whose requests look like regular browser requests.
    /// rule34.xxx (Cloudflare) rejects some non-browser clients, so the headers,
    /// the user agent and the optional cookies of the user's browser are reused.
    /// </summary>
    internal static class HttpClientHelper
    {
        #region Fields

        /// <summary>
        /// Accept header for API requests.
        /// </summary>
        public const string ApiAccept = "application/xml, text/xml, */*";

        /// <summary>
        /// Accept header for images and videos.
        /// </summary>
        public const string MediaAccept = "image/avif,image/webp,image/apng,image/*,video/*,*/*;q=0.8";

        /// <summary>
        /// User agent of a regular Chrome browser.
        /// </summary>
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/156.0.0.0 Safari/537.36";

        /// <summary>
        /// Accept-Language header of a browser.
        /// </summary>
        private const string AcceptLanguage = "en-US,en;q=0.9";

        /// <summary>
        /// sec-ch-ua header of a browser.
        /// </summary>
        private const string ClientHints = "\"Not:A-Brand\";v=\"8\", \"Chromium\";v=\"156\", \"Google Chrome\";v=\"156\"";

        /// <summary>
        /// Delay between network operations in milliseconds.
        /// rule34.xxx does not publish an exact rate limit, so the downloader is
        /// intentionally polite to the API and the CDN (two requests per second max).
        /// </summary>
        private const int RequestDelayMilliseconds = 125;

        #endregion

        #region Methods

        /// <summary>
        /// Waits <see cref="RequestDelayMilliseconds"/> before the next request.
        /// </summary>
        public static void Pause()
        {
            Thread.Sleep(RequestDelayMilliseconds);
        }

        /// <summary>
        /// Creates an HttpClient with browser-like headers.
        /// </summary>
        /// <param name="accept">Accept header value.</param>
        /// <param name="timeout">Request timeout.</param>
        /// <param name="useCookies">True to send rule34.xxx cookies (gdpr and the optional user cookies).</param>
        /// <returns>A new client with the browser-like headers.</returns>
        public static HttpClient CreateClient(string accept, TimeSpan timeout, bool useCookies)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            if (useCookies)
            {
                var container = new CookieContainer();
                AddCookie(container, "gdpr", "1");
                AddCookie(container, "gdpr-consent", "1");

                handler.UseCookies = true;
                handler.CookieContainer = container;
            }

            var client = new HttpClient(handler)
            {
                Timeout = timeout
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", accept);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", AcceptLanguage);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Pragma", "no-cache");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", ClientHints);
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Referrer = new Uri("https://rule34.xxx/");

            return client;
        }

        #endregion

        #region Helpers

        private static void AddCookie(CookieContainer container, string name, string value)
        {
            try
            {
                container.Add(new Cookie
                {
                    Name = name,
                    Value = value,
                    Domain = "rule34.xxx",
                    Path = "/",
                    Expires = DateTime.Now.AddYears(1)
                });
            }
            catch (CookieException)
            {
                // Ignore invalid cookies.
            }
        }

        #endregion
    }
}
