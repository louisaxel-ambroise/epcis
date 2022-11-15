using FasTnT.Domain.Model.Events;
using FasTnT.Features.v2_0.Communication.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Features.v2_0.Tests.Communication.Xml;

[TestClass]
public class WhenParsingAnObjectEventWithCertificationInfo : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Features.v2_0.Tests.Resources.Events.ObjectEvent_StringCertificationInfo.xml";

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
