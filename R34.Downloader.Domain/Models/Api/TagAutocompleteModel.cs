using System.Text.Json.Serialization;

namespace R34.Downloader.Domain.Models.Api
{
    /// <summary>
    /// Tag autocomplete model.
    /// </summary>
    public class TagAutocompleteModel
    {
        #region Properties

        /// <summary>
        /// Label.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; init; }

        /// <summary>
        /// Type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; init; }

        /// <summary>
        /// Value.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; init; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a string representation of the tag.
        /// </summary>
        /// <returns>Label.</returns>
        public override string ToString() => Label;

        #endregion
    }
}
