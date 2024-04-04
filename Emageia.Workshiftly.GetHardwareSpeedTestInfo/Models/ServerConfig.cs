using System.Xml.Serialization;

namespace Emageia.Workshiftly.GetHardwareSpeedTestInfo.Models
{
    [XmlRoot("server-config")]
    public class ServerConfig
    {
        [XmlAttribute("ignoreids")]
        public string IgnoreIds { get; set; }
    }
}