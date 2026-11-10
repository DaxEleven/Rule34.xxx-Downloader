using R34Downloader.Services;
using System;
using System.Text.RegularExpressions;

namespace R34Downloader.Models
{
    /// <summary>
    /// Settings DTO model.
    /// </summary>
    public static class SettingsModel
    {
        #region Properties

        /// <summary>
        /// Maximum amount of content per download. The value is intentionally capped
        /// at the 16-bit limit so that nobody downloads the whole site at once.
        /// </summary>
        public const ushort MaxLimit = ushort.MaxValue;

        /// <summary>
        /// Limit.
        /// </summary>
        public static ushort Limit { get; set; }

        /// <summary>
        /// Folder where the downloaded content is saved.
        /// </summary>
        public static string DownloadPath { get; set; } = string.Empty;

        /// <summary>
        /// Flag for images.
        /// </summary>
        public static bool Images { get; set; }

        /// <summary>
        /// Flag for GIFs.
        /// </summary>
        public static bool Gif { get; set; }

        /// <summary>
        /// Flag for videos.
        /// </summary>
        public static bool Video { get; set; }

        /// <summary>
        /// Id of the user account that owns the API key.
        /// </summary>
        public static string UserId { get; set; }

        /// <summary>
        /// Personal API key of the current user.
        /// </summary>
        public static string ApiKey { get; set; }

        /// <summary>
        /// True when both the user id and the API key are filled in.
        /// </summary>
        public static bool HasApiCredentials => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(ApiKey);

        /// <summary>
        /// Credentials in the format used by the site account page:
        /// "&amp;api_key=...&amp;user_id=...".
        /// </summary>
        public static string CredentialsLine => HasApiCredentials ? $"&api_key={ApiKey}&user_id={UserId}" : string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// Extracts the user id and the API key from a line like
        /// "&amp;api_key=...&amp;user_id=..." (the order and extra characters do not matter).
        /// </summary>
        /// <param name="value">Line pasted from the site.</param>
        /// <param name="userId">Parsed user id.</param>
        /// <param name="apiKey">Parsed API key.</param>
        /// <returns>True when both values were found.</returns>
        public static bool TryParseCredentials(string value, out string userId, out string apiKey)
        {
            userId = string.Empty;
            apiKey = string.Empty;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string text;

            try
            {
                text = Uri.UnescapeDataString(value.Trim());
            }
            catch (UriFormatException)
            {
                text = value.Trim();
            }

            var keyMatch = Regex.Match(text, @"api_key\s*=\s*([^&\s]+)", RegexOptions.IgnoreCase);
            var userMatch = Regex.Match(text, @"user_id\s*=\s*(\d+)", RegexOptions.IgnoreCase);

            if (keyMatch.Success)
            {
                apiKey = keyMatch.Groups[1].Value.Trim();
            }

            if (userMatch.Success)
            {
                userId = userMatch.Groups[1].Value.Trim();
            }

            return apiKey.Length > 0 && userId.Length > 0;
        }

        /// <summary>
        /// Loads the saved settings into the model.
        /// </summary>
        public static void Load()
        {
            PortableSettings.Load();

            DownloadPath = PortableSettings.DownloadPath;
            UserId = PortableSettings.UserId;
            ApiKey = PortableSettings.ApiKey;
        }

        /// <summary>
        /// Saves the model to the portable settings file
        /// (<c>R34Downloader.settings.xml</c> next to the executable).
        /// </summary>
        public static void Save()
        {
            PortableSettings.DownloadPath = DownloadPath;
            PortableSettings.UserId = UserId;
            PortableSettings.ApiKey = ApiKey;
            PortableSettings.Save();
        }

        #endregion
    }
}
