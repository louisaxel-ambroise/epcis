using FasTnT.Host.Communication.Xml.Parsers;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Soap;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAGetSubscriptionIDsQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Queries.GetSubscriptionIDs.xml";

    public SoapEnvelope Envelope { get; set; }

    [TestInitialize]
    public void When()
    {
        Envelope = SoapQueryParser.Parse(ParseXml(ResourceName));
    }

    [TestMethod]
    public void ItShouldReturnAListSubscriptionsRequest()
    {
        Assert.AreEqual(Envelope.Action, "GetSubscriptionIDs");
    }

    [TestMethod]
    public void TheListSubscriptionsRequestShouldHaveTheCorrectQueryName()
    {
        Assert.AreEqual("SimpleEventQuery", (Envelope.Query as ListSubscriptionsRequest).QueryName);
    }
}
