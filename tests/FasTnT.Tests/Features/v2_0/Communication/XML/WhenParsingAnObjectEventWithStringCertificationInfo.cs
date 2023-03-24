using FasTnT.Application.Domain.Format.v2_0.Parsers;
using FasTnT.Application.Domain.Model.Events;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenParsingAnObjectEventWithCertificationInfo : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Tests.Features.v2_0.Communication.Resources.Events.ObjectEvent_StringCertificationInfo.xml";

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
