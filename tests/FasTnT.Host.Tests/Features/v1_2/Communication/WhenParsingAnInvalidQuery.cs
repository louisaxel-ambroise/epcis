using FasTnT.Host.Communication.Xml.Parsers;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAnUnknownQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Queries.InvalidQuery.xml";

    [TestMethod]
    public void ItShouldReturnASoapEnvelopeWithTheCorrectAction()
    {
        var envelope = SoapQueryParser.Parse(ParseXml(ResourceName));

        Assert.IsNotNull(envelope);
        Assert.AreEqual("UnknownQuery", envelope.Action);
    }
}
