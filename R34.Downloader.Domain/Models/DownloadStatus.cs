namespace R34.Downloader.Domain.Models
{
    public class DownloadStatus
    {
        public ushort Downloaded { get; set; }

        public ushort Skipped { get; set; }
    }
}
