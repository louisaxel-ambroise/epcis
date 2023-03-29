using System.Xml.Serialization;

namespace FasTnT.Tests.Integration.v1_2.Interfaces;

[XmlRoot("GetQueryNamesResult", Namespace = "urn:epcglobal:epcis-query:xsd:1")]
public class GetQueryNamesResult
{
    [XmlElement("string")]
    public string[] Values { get; set; }
}
