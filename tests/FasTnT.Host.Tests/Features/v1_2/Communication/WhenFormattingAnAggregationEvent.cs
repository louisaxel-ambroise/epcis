using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v1_2.Communication.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication.XML;

[TestClass]
public class WhenFormattingAnAggregationEvent
{
    public Event AggregationEvent { get; set; }
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        AggregationEvent = new Event
        {
            Type = EventType.AggregationEvent,
            EventTimeZoneOffset = -180,
            BusinessStep = "step",
            Disposition = "testDisp",
            BusinessLocation = "loc",
            ReadPoint = "readPointTest",
            EventId = "ni://test",
            Action = EventAction.Add,
            Epcs = new List<Epc> { new Epc { Type = EpcType.ParentId, Id = "test:epc:parent" }, new Epc { Type = EpcType.ChildEpc, Id = "test:childepc" }, new Epc { Type = EpcType.Quantity, Id = "test:chqty" } }
        };

        Formatted = XmlEventFormatter.FormatList(new List<Event> { AggregationEvent }).FirstOrDefault();
    }

    [TestMethod]
    public void ItShouldFormatTheEventToXml()
    {
        Assert.IsNotNull(Formatted);
        Assert.AreEqual("AggregationEvent", Formatted.Name.LocalName);
    }

    [TestMethod]
    public void ItShouldFormatTheEventCorrectly()
    {
        Assert.AreEqual(AggregationEvent.EventTimeZoneOffset.Representation, Formatted.Element("eventTimeZoneOffset").Value);
        Assert.AreEqual(AggregationEvent.EventId, Formatted.Element("baseExtension").Element("eventID").Value);
        Assert.AreEqual(AggregationEvent.Action.ToString().ToUpper(), Formatted.Element("action").Value);
        Assert.AreEqual(AggregationEvent.Epcs.Single(x => x.Type == EpcType.ParentId).Id, Formatted.Element("parentID").Value);
        Assert.AreEqual(AggregationEvent.BusinessStep, Formatted.Element("bizStep").Value);
        Assert.AreEqual(AggregationEvent.Disposition, Formatted.Element("disposition").Value);
        Assert.AreEqual(AggregationEvent.Epcs.Count(x => x.Type == EpcType.ChildEpc), Formatted.Element("childEPCs").Elements().Count());
        Assert.AreEqual(AggregationEvent.Epcs.Count(x => x.Type == EpcType.Quantity), Formatted.Element("extension").Element("childQuantityList").Elements().Count());
        Assert.AreEqual(AggregationEvent.ReadPoint, Formatted.Element("readPoint").Element("id").Value);
        Assert.AreEqual(AggregationEvent.BusinessLocation, Formatted.Element("bizLocation").Element("id").Value);
        Assert.AreEqual(0, Formatted.Elements().Where(x => x.Name.NamespaceName != string.Empty).Count());
    }
}
