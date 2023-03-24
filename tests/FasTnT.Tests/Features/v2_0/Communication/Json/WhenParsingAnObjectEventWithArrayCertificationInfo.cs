using FasTnT.Application.Domain.Format.v2_0.Parsers;
using FasTnT.Application.Domain.Model.Events;

namespace FasTnT.Tests.Features.v2_0.Communication.Json;

[TestClass]
public class WhenParsingAnObjectEventWithArrayCertificationInfo : JsonParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Tests.Features.v2_0.Communication.Resources.Events.ObjectEvent_StringCertificationInfoArray.json";

    public Event Event { get; set; }

    [TestInitialize]
    public void When()
    {
        var parser = JsonEventParser.Create(ParseResource(ResourceName).RootElement, TestNamespaces.Default);
        Event = parser.Parse();
    }

    [TestMethod]
    public void CertificationInfoShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:fastnt:cert:115120.021", Event.CertificationInfo);
    }
}
