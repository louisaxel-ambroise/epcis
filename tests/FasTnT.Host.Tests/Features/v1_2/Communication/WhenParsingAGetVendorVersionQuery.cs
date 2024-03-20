using FasTnT.Host.Communication.Xml.Parsers;
using FasTnT.Host.Endpoints.Responses.Soap;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAGetVendorVersionQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Queries.GetVendorVersion.xml";

    public SoapEnvelope Envelope { get; set; }

    [TestInitialize]
    public void When()
    {
        Envelope = SoapQueryParser.Parse(ParseXml(ResourceName));
    }

    [TestMethod]
    public void ItShouldReturnAGetStandardVersionObject()
    {
        Assert.AreEqual(Envelope.Action, "GetVendorVersion");
    }
}
