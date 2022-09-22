using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Infrastructure.Utils;
using FasTnT.Domain.Model.Events;
using FasTnT.Features.v1_2.Communication.Parsers;

namespace FasTnT.Features.v1_2.Tests;

[TestClass]
public class WhenParsingAValidAggregationEvent : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Features.v1_2.Tests.Resources.Events.AggregationEvent_Full.xml";

    public Event Event { get; set; }

    [TestInitialize]
    public void When()
    {
        Event = XmlEventParser.ParseAggregationEvent(ParseResource(ResourceName).Root);
    }

    [TestMethod]
    public void ActionShouldBeParsedCorrectly()
    {
        Assert.AreEqual(EventAction.Add, Event.Action);
    }

    [TestMethod]
    public void EventTimeShouldBeParsedCorrectly()
    {
        var expectedDate = new DateTime(2013, 06, 08, 14, 58, 56, 591, DateTimeKind.Utc);
        Assert.AreEqual(expectedDate, Event.EventTime);
    }

    [TestMethod]
    public void EventTimeZoneOffsetShouldBeParsedCorrectly()
    {
        var expectedOffset = new TimeZoneOffset { Representation = "+02:00" };
        Assert.IsNotNull(Event.EventTimeZoneOffset);
        Assert.AreEqual(expectedOffset.Value, Event.EventTimeZoneOffset.Value);
    }

    [TestMethod]
    public void BizStepShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epcglobal:cbv:bizstep:receiving", Event.BusinessStep);
    }

    [TestMethod]
    public void DispositionShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epcglobal:cbv:disp:in_progress", Event.Disposition);
    }

    [TestMethod]
    public void ReadPointShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epc:id:sgln:0614141.00777.0", Event.ReadPoint);
    }

    [TestMethod]
    public void BizLocationShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epc:id:sgln:0614141.00888.0", Event.BusinessLocation);
    }

    [TestMethod]
    public void ExtensionFieldsShouldBeParsedCorrectly()
    {
        Assert.AreEqual(1, Event.Fields.Where(x => x.Type == FieldType.CustomField).Count());
    }

    [TestMethod]
    public void EventShouldContainsParentIdEpcs()
    {
        Assert.IsNotNull(Event.Epcs.SingleOrDefault(x => x.Type == EpcType.ParentId));
        Assert.AreEqual("urn:epc:id:sscc:0614141.1234567890", Event.Epcs.Single(x => x.Type == EpcType.ParentId).Id);
    }

    [TestMethod]
    public void EventShouldContainsChildEpcs()
    {
        Assert.AreEqual(2, Event.Epcs.Count(x => x.Type == EpcType.ChildEpc));
        Assert.IsTrue(Event.Epcs.Any(x => x.Type == EpcType.ChildEpc && x.Id == "urn:epc:id:sgtin:0614141.107346.2017"));
        Assert.IsTrue(Event.Epcs.Any(x => x.Type == EpcType.ChildEpc && x.Id == "urn:epc:id:sgtin:0614141.107346.2018"));
    }

    [TestMethod]
    public void EventShouldContainsChildQuantityEpcs()
    {
        Assert.AreEqual(2, Event.Epcs.Count(x => x.Type == EpcType.ChildQuantity));
        Assert.IsTrue(Event.Epcs.Any(x => x.Type == EpcType.ChildQuantity && x.Id == "urn:epc:idpat:sgtin:4012345.098765.*" && x.Quantity == 10));
        Assert.IsTrue(Event.Epcs.Any(x => x.Type == EpcType.ChildQuantity && x.Id == "urn:epc:class:lgtin:4012345.012345.998877" && x.Quantity == 200.5 && x.UnitOfMeasure == "KGM"));
    }
}
