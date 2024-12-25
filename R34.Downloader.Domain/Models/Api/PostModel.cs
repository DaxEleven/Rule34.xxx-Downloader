using System.Xml.Serialization;

namespace R34.Downloader.Domain.Models.Api
{
    [XmlRoot(ElementName = "post")]
    public class PostModel
    {
        [XmlAttribute(AttributeName = "id")]
        public long Id { get; set; }

        [XmlAttribute(AttributeName = "file_url")]
        public string FileUrl { get; set; }

        [XmlAttribute(AttributeName = "sample_url")]
        public string SampleUrl { get; set; }

        [XmlAttribute(AttributeName = "created_at")]
        public string CreatedAt { get; set; }
    }
}
