using FasTnT.Host.Communication.Xml.Parsers;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Soap;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAnUnsubscribeQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Queries.Unsubscribe.xml";

    public SoapEnvelope Envelope { get; set; }

    [TestInitialize]
    public void When()
    {
        Envelope = SoapQueryParser.Parse(ParseXml(ResourceName));
    }

    [TestMethod]
    public void ItShouldReturnAnUnsubscribeObject()
    {
        Assert.AreEqual(Envelope.Action, "Unsubscribe");
    }

    [TestMethod]
    public void TheUnsubscribeCommandShouldHaveTheCorrectSubscriptionId()
    {
        Assert.AreEqual("TestSubscription", (Envelope.Query as Unsubscribe).SubscriptionId);
    }
}
