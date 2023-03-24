using System.Xml.Serialization;

namespace FasTnT.IntegrationTests.v1_2.Interfaces;

[XmlRoot("GetStandardVersionResult", Namespace = "urn:epcglobal:epcis-query:xsd:1")]
public class GetStandardVersionResult
{
    public string Version { get; set; }
}
