using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace R34Downloader.Services
{
    /// <summary>
    /// Shared retry policy for HTTP requests: transient status codes, exponential
    /// backoff with jitter, the Retry-After header and a single status callback.
    /// </summary>
    internal static class RetryPolicy
    {
        #region Fields

        /// <summary>
        /// Maximum number of attempts for a single request.
        /// </summary>
        public const int MaxAttempts = 5;

        private const int BaseDelayMilliseconds = 1000;
        private const int MaxDelayMilliseconds = 30000;

        #endregion

        #region Properties

        /// <summary>
        /// Called before a retry (used to update the status bar). May be null.
        /// </summary>
        public static Action<string> OnRetryMessage { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// True for status codes that can be retried: 408, 429 and 5xx.
        /// </summary>
        /// <param name="statusCode">Status code of the response.</param>
        /// <returns>True when the request may be retried.</returns>
        public static bool IsTransient(HttpStatusCode statusCode)
        {
            var code = (int)statusCode;

            return code == 408 || code == 429 || code >= 500;
        }

        /// <summary>
        /// True for exceptions that usually mean a temporary network problem.
        /// </summary>
        /// <param name="exception">Exception to inspect, including its inner exceptions.</param>
        /// <returns>True when the exception looks like a temporary network failure.</returns>
        public static bool IsTransient(Exception exception)
        {
            for (var current = exception; current != null; current = current.InnerException)
            {
                if (current is HttpRequestException || current is TaskCanceledException || current is IOException)
                {
                    return true;
                }

                if (current is WebException webException)
                {
                    if (!(webException.Response is HttpWebResponse webResponse) || IsTransient(webResponse.StatusCode))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the delay requested by the server (Retry-After) or the backoff delay.
        /// </summary>
        /// <param name="response">Response that contains the Retry-After header. May be null.</param>
        /// <param name="attempt">Number of the attempt that has just failed (one based).</param>
        /// <returns>Delay before the next attempt.</returns>
        public static TimeSpan GetDelay(HttpResponseMessage response, int attempt)
        {
            var retryAfter = response?.Headers.RetryAfter;
            if (retryAfter != null)
            {
                if (retryAfter.Delta.HasValue)
                {
                    return Clamp(retryAfter.Delta.Value);
                }

                if (retryAfter.Date.HasValue)
                {
                    return Clamp(retryAfter.Date.Value - DateTimeOffset.Now);
                }
            }

            return GetBackoff(attempt);
        }

        /// <summary>
        /// Exponential backoff: 1, 2, 4, 8, 16, 32 (capped at 30) seconds with +/-20% jitter.
        /// </summary>
        /// <param name="attempt">Number of the attempt that has just failed (one based).</param>
        /// <returns>Delay with jitter for the given attempt.</returns>
        public static TimeSpan GetBackoff(int attempt)
        {
            var exponent = Math.Min(Math.Max(attempt - 1, 0), 5);
            var delay = Math.Min(BaseDelayMilliseconds * (int)Math.Pow(2, exponent), MaxDelayMilliseconds);

            var jitterRange = Math.Max(1, delay / 5);
            var jitter = (Environment.TickCount & 0x7FFFFFFF) % (jitterRange * 2 + 1) - jitterRange;

            return TimeSpan.FromMilliseconds(Math.Max(500, delay + jitter));
        }

        /// <summary>
        /// Reports a retry message to the UI; a failed update must not break the download.
        /// </summary>
        /// <param name="message">Message for the status bar.</param>
        public static void Report(string message)
        {
            try
            {
                OnRetryMessage?.Invoke(message);
            }
            catch
            {
                // Ignored on purpose.
            }
        }

        #endregion

        #region Helpers

        private static TimeSpan Clamp(TimeSpan delay)
        {
            if (delay < TimeSpan.FromSeconds(1))
            {
                return TimeSpan.FromSeconds(1);
            }

            if (delay > TimeSpan.FromMinutes(2))
            {
                return TimeSpan.FromMinutes(2);
            }

            return delay;
        }

        #endregion
    }
}
