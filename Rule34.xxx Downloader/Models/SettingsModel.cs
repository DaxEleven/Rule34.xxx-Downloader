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
    }
}