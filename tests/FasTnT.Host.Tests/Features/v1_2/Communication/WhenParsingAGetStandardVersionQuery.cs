using FasTnT.Host.Communication.Xml.Parsers;
using FasTnT.Host.Endpoints.Responses.Soap;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAGetStandardVersionQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Queries.GetStandardVersion.xml";

    public SoapEnvelope Query { get; set; }

    [TestInitialize]
    public void When()
    {
        Query = SoapQueryParser.Parse(ParseXml(ResourceName));
    }

    [TestMethod]
    public void ItShouldReturnAGetStandardVersionObject()
    {
        Assert.AreEqual(Query.Action, "GetStandardVersion");
    }
}
