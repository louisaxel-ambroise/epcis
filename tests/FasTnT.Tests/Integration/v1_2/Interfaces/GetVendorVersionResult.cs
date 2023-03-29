using System.Xml.Serialization;

namespace FasTnT.Tests.Integration.v1_2.Interfaces;

[XmlRoot("GetVendorVersionResult", Namespace = "urn:epcglobal:epcis-query:xsd:1")]
public class GetVendorVersionResult
{
    public string Version { get; set; }
}
