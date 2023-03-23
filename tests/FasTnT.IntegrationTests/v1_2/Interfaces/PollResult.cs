using System.Xml.Serialization;

namespace FasTnT.IntegrationTests.v1_2.Interfaces;

[XmlRoot("QueryResults", Namespace = "urn:epcglobal:epcis-query:xsd:1")]
public class QueryResults
{
    public string QueryName { get; set; }
}
