using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Features.v2_0.Communication.Json.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FasTnT.Features.v2_0.Tests.Communication.Json;

[TestClass]
public class WhenParsingAValidTransformationEvent : JsonParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Features.v2_0.Tests.Resources.Events.TransformationEvent_Full.json";

    public Event Event { get; set; }

    [TestInitialize]
    public void When()
    {
        var parser = JsonEventParser.Create(ParseResource(ResourceName).RootElement, TestNamespaces.Default);
        Event = parser.Parse();
    }

    [TestMethod]
    public void EventTimeShouldBeParsedCorrectly()
    {
        var expectedDate = new DateTime(2013, 10, 31, 14, 58, 56, DateTimeKind.Utc);
        Assert.AreEqual(expectedDate, Event.EventTime);
    }

    [TestMethod]
    public void EventTimeZoneOffsetShouldBeParsedCorrectly()
    {
        Assert.IsNotNull(Event.EventTimeZoneOffset);
        Assert.AreEqual("+02:00", Event.EventTimeZoneOffset.Representation);
    }

    [TestMethod]
    public void BizStepShouldBeParsedCorrectly()
    {
        Assert.AreEqual("commissioning", Event.BusinessStep);
    }

    [TestMethod]
    public void DispositionShouldBeParsedCorrectly()
    {
        Assert.AreEqual("in_progress", Event.Disposition);
    }

    [TestMethod]
    public void TransformationIdShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epc:id:gdti:0614141.12345.400", Event.TransformationId);
    }

    [TestMethod]
    public void ReadPointShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epc:id:sgln:4012345.00001.0", Event.ReadPoint);
    }

    [TestMethod]
    public void BizLocationShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epc:id:sgln:0614141.00888.0", Event.BusinessLocation);
    }

    [TestMethod]
    public void BizTransactionsShouldBeParsedCorrectly()
    {
        Assert.AreEqual(1, Event.Transactions.Count);
        Assert.IsTrue(Event.Transactions.Any(x => x.Type == "po" && x.Id == "urn:epc:id:gdti:0614141.00001.1618034"));
    }

    [TestMethod]
    public void SourcesShouldBeParsedCorrectly()
    {
        Assert.AreEqual(2, Event.Sources.Count);
        Assert.IsTrue(Event.Sources.Any(x => x.Type == "location" && x.Id == "urn:epc:id:sgln:4012345.00225.0"));
        Assert.IsTrue(Event.Sources.Any(x => x.Type == "possessing_party" && x.Id == "urn:epc:id:pgln:4012345.00225"));
    }

    [TestMethod]
    public void DestinationsShouldBeParsedCorrectly()
    {
        Assert.AreEqual(1, Event.Destinations.Count);
        Assert.IsTrue(Event.Destinations.Any(x => x.Type == "location" && x.Id == "urn:epc:id:sgln:0614141.00777.0"));
    }

    [TestMethod]
    public void PersistentDispositionsShouldBeParsedCorrectly()
    {
        Assert.AreEqual(2, Event.PersistentDispositions.Count);
        Assert.IsTrue(Event.PersistentDispositions.Any(x => x.Type == PersistentDispositionType.Set && x.Id == "completeness_verified"));
        Assert.IsTrue(Event.PersistentDispositions.Any(x => x.Type == PersistentDispositionType.Unset && x.Id == "completeness_inferred"));
    }

    [TestMethod]
    public void EventShouldContainsAllEpcListAndQuantities()
    {
        Assert.AreEqual(8, Event.Epcs.Count);

        Assert.IsTrue(Event.Epcs.Any(e => e.Id == "urn:epc:id:sgtin:4012345.011122.25" && e.Type == EpcType.InputEpc && !e.IsQuantity), "EPC urn:epc:id:sscc:4001356.5900000817 is expected");
        Assert.IsTrue(Event.Epcs.Any(e => e.Id == "urn:epc:id:sgtin:4000001.065432.99886655" && e.Type == EpcType.InputEpc && !e.IsQuantity), "EPC urn:epc:id:sgtin:4000001.065432.99886655 is expected");
        Assert.IsTrue(Event.Epcs.Any(e => e.Id == "urn:epc:class:lgtin:4012345.011111.4444" && e.Type == EpcType.InputQuantity && e.IsQuantity && e.Quantity == 10 && e.UnitOfMeasure == "KGM"), "EPC urn:epc:class:lgtin:4012345.011111.4444 is expected");
        Assert.IsTrue(Event.Epcs.Any(e => e.Id == "urn:epc:class:lgtin:0614141.077777.987" && e.Type == EpcType.InputQuantity && e.IsQuantity && e.Quantity == 30), "EPC urn:epc:class:lgtin:0614141.077777.987 is expected");
        Assert.IsTrue(Event.Epcs.Any(e => e.Id == "urn:epc:id:sgtin:4012345.077889.25" && e.Type == EpcType.OutputEpc && !e.IsQuantity), "EPC urn:epc:id:sgtin:4012345.077889.25 is expected");
        Assert.IsTrue(Event.Epcs.Any(e => e.Id == "urn:epc:id:sgtin:4012345.077889.26" && e.Type == EpcType.OutputEpc && !e.IsQuantity), "EPC urn:epc:id:sgtin:4012345.077889.26 is expected");
        Assert.IsTrue(Event.Epcs.Any(e => e.Id == "urn:epc:class:lgtin:4012345.011111.4444" && e.Type == EpcType.OutputQuantity && e.IsQuantity && e.Quantity == 10 && e.UnitOfMeasure == "KGM"), "EPC urn:epc:class:lgtin:4012345.011111.4444 is expected");
        Assert.IsTrue(Event.Epcs.Any(e => e.Id == "urn:epc:class:lgtin:0614141.077777.987" && e.Type == EpcType.OutputQuantity && e.IsQuantity && e.Quantity == 30), "EPC urn:epc:class:lgtin:0614141.077777.987 is expected");
    }

    [TestMethod]
    public void CustomFieldsShouldBeParsedCorrectly()
    {
        Assert.AreEqual(2, Event.Fields.Count);
        Assert.IsTrue(Event.Fields.Any(x => x.Type == FieldType.CustomField && x.Namespace == "http://ns.example.com/ext1/" && x.Name == "int" && x.TextValue == "10"));
        Assert.IsTrue(Event.Fields.Any(x => x.Type == FieldType.CustomField && x.Namespace == "http://ns.example.com/ext1/" && x.Name == "string" && x.TextValue == "string"));
    }
}
