using System.IO;

namespace R34Downloader.Models
{
    /// <summary>
    /// rule34.xxx post DTO.
    /// </summary>
    public class PostModel
    {
        /// <summary>
        /// Post id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Original file url.
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Resized sample url (for videos it is a still image, not the video itself).
        /// </summary>
        public string SampleUrl { get; set; }

        /// <summary>
        /// Preview url.
        /// </summary>
        public string PreviewUrl { get; set; }

        /// <summary>
        /// Url that should be downloaded.
        /// </summary>
        public string DownloadUrl => string.IsNullOrWhiteSpace(FileUrl) ? SampleUrl : FileUrl;

        /// <summary>
        /// File extension (with a dot, in lower case).
        /// </summary>
        public string Extension
        {
            get
            {
                var url = DownloadUrl;

                if (string.IsNullOrWhiteSpace(url))
                {
                    return string.Empty;
                }

                var queryIndex = url.IndexOf('?');
                var path = queryIndex >= 0 ? url.Substring(0, queryIndex) : url;
                var slashIndex = path.LastIndexOf('/');
                var fileName = slashIndex >= 0 ? path.Substring(slashIndex + 1) : path;

                return Path.GetExtension(fileName).ToLowerInvariant();
            }
        }

        /// <summary>
        /// True for video posts.
        /// </summary>
        public bool IsVideo => Extension == ".mp4" || Extension == ".webm";

        /// <summary>
        /// True for gif posts.
        /// </summary>
        public bool IsGif => Extension == ".gif";
    }
}
