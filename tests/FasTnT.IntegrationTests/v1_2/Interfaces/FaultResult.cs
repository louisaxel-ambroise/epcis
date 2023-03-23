using System.Xml.Serialization;

namespace FasTnT.IntegrationTests.v1_2.Interfaces;

[XmlRoot("Fault", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class FaultResult
{
    public string Reason { get; set; }
    public string Severity { get; set; }
    public string QueryName { get; set; }
    public string SubscriptionId { get; set; }
}
