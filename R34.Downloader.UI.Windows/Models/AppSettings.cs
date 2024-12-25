using System.Text.Json.Serialization;

namespace R34.Downloader.UI.Windows.Models
{
    internal class AppSettings
    {
        #region Properties

        /// <summary>
        /// Content save path.
        /// </summary>
        [JsonPropertyName("savePath")]
        public string SavePath { get; init; }

        #endregion
    }
}
