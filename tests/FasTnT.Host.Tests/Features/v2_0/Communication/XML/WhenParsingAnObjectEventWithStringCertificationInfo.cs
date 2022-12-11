using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication.Xml.Parsers;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenParsingAnObjectEventWithCertificationInfo : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v2_0.Communication.Resources.Events.ObjectEvent_StringCertificationInfo.xml";

    public Event Event { get; set; }

    [TestInitialize]
    public void When()
    {
        Event = XmlEventParser.ParseEvent(ParseResource(ResourceName));
    }

    [TestMethod]
    public void CertificationInfoShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:fastnt:cert:115120.021", Event.CertificationInfo);
    }
}
