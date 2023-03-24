using FasTnT.Application.Domain.Model;
using FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.Json;

[TestClass]
public class WhenParsingARequestContainingEventAndCbvMasterdata : JsonParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v2_0.Communication.Resources.Requests.Request_ObjectEvent_WithMasterdata.json";

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
    public void RequestShouldContainsOneMasterdata()
    {
        Assert.AreEqual(1, Request.Masterdata.Count);
    }

    [TestMethod]
    public void ParsedMasterdataShouldMatchTheJsonDocument()
    {
        var masterdata = Request.Masterdata.Single();

        Assert.AreEqual("urn:epcglobal:epcis:vtype:ReadPoint", masterdata.Type);
        Assert.AreEqual("urn:epc:id:sgln:0037000.00729.8000", masterdata.Id);
        Assert.AreEqual(1, masterdata.Attributes.Count);
        Assert.AreEqual("urn:epcglobal:cbv:mda:site", masterdata.Attributes.Single().Id);
        Assert.AreEqual("test", masterdata.Attributes.Single().Value);
        Assert.AreEqual(0, masterdata.Attributes.Single().Fields.Count);
    }

    [TestMethod]
    public void RequestDateShouldBePopulated()
    {
        var expectedDate = new DateTime(2013, 06, 04, 12, 59, 02, 099);
        Assert.AreEqual(expectedDate, Request.DocumentTime);
    }
}
