using System;

namespace R34Downloader.Services
{
    /// <summary>
    /// Exception that contains a user-friendly description of a rule34.xxx error.
    /// </summary>
    public class R34Exception : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the R34Exception class.
        /// </summary>
        /// <param name="message">Error description.</param>
        public R34Exception(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the R34Exception class.
        /// </summary>
        /// <param name="message">Error description.</param>
        /// <param name="innerException">Inner exception.</param>
        public R34Exception(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }
}
