using System;
using System.Web;

namespace R34.Downloader.Domain.Models
{
    /// <summary>
    /// 
    /// </summary>
    public record SearchPhrase
    {
        #region Fields

        private string Value { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SearchPhrase class.
        /// </summary>
        public SearchPhrase()
        {
            Value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the SearchPhrase class.
        /// </summary>
        /// <param name="searchText"></param>
        public SearchPhrase(string searchText)
        {
            Value = searchText != null ? HttpUtility.UrlEncode(searchText.Trim()) : string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a string representation of the search phrase.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString() => Value;

        #endregion
    }
}
