namespace R34.Downloader.Domain.Models
{
    /// <summary>
    /// Settings model.
    /// </summary>
    public class Settings
    {
        #region Properties

        /// <summary>
        /// Search tags.
        /// </summary>
        public SearchPhrase SearchPhrase { get; set; }

        /// <summary>
        /// Whether to include images in downloaded content.
        /// </summary>
        public bool IncludeImages { get; set; }

        /// <summary>
        /// Whether to include gifs in downloaded content.
        /// </summary>
        public bool IncludeGifs { get; set; }

        /// <summary>
        /// Whether to include video in downloaded content.
        /// </summary>
        public bool IncludeVideo { get; set; }

        /// <summary>
        /// The amount of downloaded content.
        /// </summary>
        /// <remarks>Maximum 65535.</remarks>
        public ushort Quantity { get; set; }

        /// <summary>
        /// Download content in maximum quality.
        /// </summary>
        public bool DownloadContentInMaxQuality { get; set; }

        /// <summary>
        /// Content save path.
        /// </summary>
        public string SavePath { get; set; }

        #endregion
    }
}
