using System.Xml.Serialization;

namespace R34.Downloader.Domain.Models.Api
{
    [XmlRoot(ElementName = "posts")]
    public class PostsModel
    {
        [XmlElement(ElementName = "post")]
        public PostModel[] Posts { get; set; }

        [XmlAttribute(AttributeName = "count")]
        public uint Count { get; set; }

        [XmlAttribute(AttributeName = "offset")]
        public uint Offset { get; set; }
    }
}
