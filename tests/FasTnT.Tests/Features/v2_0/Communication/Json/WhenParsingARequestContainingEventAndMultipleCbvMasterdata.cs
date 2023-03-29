using FasTnT.Host.Features.v2_0.Communication.Parsers;

namespace FasTnT.Tests.Features.v2_0.Communication.Json;

[TestClass]
public class WhenParsingARequestContainingEventAndMultipleCbvMasterdata : JsonParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Tests.Features.v2_0.Communication.Resources.Requests.Request_ObjectEvent_WithMultipleMasterdata.json";

    public Request Request { get; set; }

    [TestInitialize]
    public void When()
    {
        Request = JsonEpcisDocumentParser.Parse(ParseResource(ResourceName), TestNamespaces.Default);
    }

    [TestMethod]
    public void RequestShouldContainsOneEvent()
    {
        Assert.AreEqual(1, Request.Events.Count);
    }

    [TestMethod]
    public void RequestShouldContainsAllTheMasterdata()
    {
        Assert.AreEqual(3, Request.Masterdata.Count);
        Assert.AreEqual(2, Request.Masterdata.Count(x => x.Type == "urn:epcglobal:epcis:vtype:ReadPoint"));
        Assert.AreEqual(1, Request.Masterdata.Count(x => x.Type == "urn:epcglobal:epcis:vtype:BizLocation"));
        Assert.AreEqual(2, Request.Masterdata.Single(x => x.Type == "urn:epcglobal:epcis:vtype:BizLocation").Attributes.Count);
        Assert.AreEqual(2, Request.Masterdata.Single(x => x.Type == "urn:epcglobal:epcis:vtype:BizLocation").Children.Count);
    }

    [TestMethod]
    public void RequestDateShouldBePopulated()
    {
        var expectedDate = new DateTime(2013, 06, 04, 12, 59, 02, 99);
        Assert.AreEqual(expectedDate, Request.DocumentTime);
    }
}
