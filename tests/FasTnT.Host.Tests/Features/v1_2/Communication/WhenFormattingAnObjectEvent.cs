using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAnObjectEvent
{
    public Event ObjectEvent { get; set; }
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        ObjectEvent = new Event
        {
            Type = EventType.ObjectEvent,
            EventTimeZoneOffset = "+02:00",
            BusinessStep = "step",
            Disposition = "testDisp",
            BusinessLocation = "loc",
            ReadPoint = "readPointTest",
            EventId = "ni://test",
            Action = EventAction.Add,
            Epcs = [new Epc { Type = EpcType.List, Id = "test:epc" }],
            Fields = [new Field { Type = FieldType.Ilmd, Name = "field", Namespace = "fastnt.ns", TextValue = "OK" }],
            Request = new Domain.Model.Request { RecordTime = DateTime.Now }
        };

        Formatted = XmlV1EventFormatter.Instance.FormatList(new List<Event> { ObjectEvent }).FirstOrDefault();
    }

    [TestMethod]
    public void ItShouldFormatTheEventToXml()
    {
        Assert.IsNotNull(Formatted);
        Assert.AreEqual("ObjectEvent", Formatted.Name.LocalName);
    }

    [TestMethod]
    public void ItShouldFormatTheEventCorrectly()
    {
        Assert.AreEqual(ObjectEvent.EventTimeZoneOffset.Representation, Formatted.Element("eventTimeZoneOffset").Value);
        Assert.AreEqual(ObjectEvent.EventId, Formatted.Element("baseExtension").Element("eventID").Value);
        Assert.AreEqual(ObjectEvent.Action.ToString().ToUpper(), Formatted.Element("action").Value);
        Assert.AreEqual(ObjectEvent.BusinessStep, Formatted.Element("bizStep").Value);
        Assert.AreEqual(ObjectEvent.Disposition, Formatted.Element("disposition").Value);
        Assert.AreEqual(ObjectEvent.Epcs.Count(x => x.Type == EpcType.List), Formatted.Element("epcList").Elements().Count());
        Assert.AreEqual(ObjectEvent.ReadPoint, Formatted.Element("readPoint").Element("id").Value);
        Assert.AreEqual(ObjectEvent.BusinessLocation, Formatted.Element("bizLocation").Element("id").Value);
        Assert.AreEqual(1, Formatted.Element("extension").Element("ilmd").Elements().Count());
    }
}
