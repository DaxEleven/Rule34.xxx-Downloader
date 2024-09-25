namespace R34Downloader.Models
{
    /// <summary>
    /// Settings DTO model.
    /// </summary>
    public static class SettingsModel
    {
        /// <summary>
        /// Limit
        /// </summary>
        public static ushort Limit { get; set; }

        /// <summary>
        /// Flag for images.
        /// </summary>
        public static bool Images { get; set; }

        /// <summary>
        /// Flag for gifs.
        /// </summary>
        public static bool Gif { get; set; }

        /// <summary>
        /// Flag for videos.
        /// </summary>
        public static bool Video { get; set; }

        /// <summary>
        /// Parsing method flag.
        /// </summary>
        public static bool IsApi { get; set; }

        /// <summary>
        /// Aria2 server URL.
        /// </summary>
        public static string Aria2ServerUrl { get; set; }

        /// <summary>
        /// Aria2 secret token for authentication.
        /// </summary>
        public static string Aria2SecretToken { get; set; }

        /// <summary>
        /// Flag for enabling or disabling aria2.
        /// </summary>
        public static bool UseAria2 { get; set; }
    }
}