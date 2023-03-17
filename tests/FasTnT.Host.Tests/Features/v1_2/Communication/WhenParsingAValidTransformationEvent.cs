using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v1_2.Communication.Parsers;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAValidTransformationEvent : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Events.TransformationEvent_Full.xml";

    public Event Event { get; set; }

    [TestInitialize]
    public void When()
    {
        Event = new XmlEventParser().ParseTransformationEvent(ParseResource(ResourceName).Root);
    }

    [TestMethod]
    public void ActionShouldBeParsedCorrectly()
    {
        Assert.AreEqual(EventAction.None, Event.Action);
    }

    [TestMethod]
    public void EventTimeShouldBeParsedCorrectly()
    {
        var expectedDate = new DateTime(2013, 10, 31, 14, 58, 56, 591);
        Assert.AreEqual(expectedDate, Event.EventTime);
    }

    [TestMethod]
    public void EventTimeZoneOffsetShouldBeParsedCorrectly()
    {
        var expectedOffset = new TimeZoneOffset { Representation = "-02:00" };
        Assert.IsNotNull(Event.EventTimeZoneOffset);
        Assert.AreEqual(expectedOffset.Value, Event.EventTimeZoneOffset.Value);
    }

    [TestMethod]
    public void BizStepShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epcglobal:cbv:bizstep:commissioning", Event.BusinessStep);
    }

    [TestMethod]
    public void DispositionShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epcglobal:cbv:disp:in_progress", Event.Disposition);
    }

    [TestMethod]
    public void ReadPointShouldBeParsedCorrectly()
    {
        Assert.AreEqual("urn:epc:id:sgln:4012345.00001.0", Event.ReadPoint);
    }

    [TestMethod]
    public void IlmdShouldBeParsedCorrectly()
    {
        Assert.AreEqual(2, Event.Fields.Count(x => x.Type == FieldType.Ilmd));

        Assert.IsTrue(Event.Fields.Any(x => x.Type == FieldType.Ilmd && x.Name == "bestBeforeDate" && x.Namespace == "http://ns.example.com/epcis" && x.TextValue == "2014-12-10"));
        Assert.IsTrue(Event.Fields.Any(x => x.Type == FieldType.Ilmd && x.Name == "batch" && x.Namespace == "http://ns.example.com/epcis" && x.TextValue == "XYZ"));
    }

    [TestMethod]
    public void ExtensionFieldsShouldBeParsedCorrectly()
    {
        Assert.AreEqual(1, Event.Fields.Where(x => x.Type == FieldType.CustomField).Count());
    }
}
