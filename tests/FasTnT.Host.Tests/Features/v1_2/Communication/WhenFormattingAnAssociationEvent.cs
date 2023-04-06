using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v1_2.Communication.Formatters;

using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication.XML;

[TestClass]
public class WhenFormattingAnAssociationEvent
{
    public Event AssociationEvent { get; set; }
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        AssociationEvent = new Event
        {
            Type = EventType.AssociationEvent,
            EventTimeZoneOffset = -180,
            BusinessStep = "step",
            Disposition = "testDisp",
            BusinessLocation = "loc",
            ReadPoint = "readPointTest",
            EventId = "ni://test",
            Action = EventAction.Add,
            Epcs = new List<Epc> { new Epc { Type = EpcType.ParentId, Id = "test:epc:parent" }, new Epc { Type = EpcType.ChildEpc, Id = "test:childepc" } },
            Fields = new List<Field> { new Field { Type = FieldType.Ilmd, Name = "field", Namespace = "fastnt.ns", TextValue = "OK"} },
            Request = new Domain.Model.Request { RecordTime = DateTime.Now }
        };

        Formatted = XmlEventFormatter.FormatList(new List<Event> { AssociationEvent }).FirstOrDefault();
    }

    [TestMethod]
    public void ItShouldFormatTheEventToXml()
    {
        Assert.IsNotNull(Formatted);
        Assert.AreEqual("extension", Formatted.Name.LocalName);
        Assert.AreEqual("extension", Formatted.Elements().First().Name.LocalName);
        Assert.AreEqual("AssociationEvent", Formatted.Elements().First().Elements().First().Name.LocalName);
    }

    [TestMethod]
    public void ItShouldFormatTheEventCorrectly()
    {
        var eventElement = Formatted.Elements().First().Elements().First();

        Assert.AreEqual(AssociationEvent.EventTimeZoneOffset.Representation, eventElement.Element("eventTimeZoneOffset").Value);
        Assert.AreEqual(AssociationEvent.EventId, eventElement.Element("baseExtension").Element("eventID").Value);
        Assert.AreEqual(AssociationEvent.Action.ToString().ToUpper(), eventElement.Element("action").Value);
        Assert.AreEqual(AssociationEvent.Epcs.Single(x => x.Type == EpcType.ParentId).Id, eventElement.Element("parentID").Value);
        Assert.AreEqual(AssociationEvent.BusinessStep, eventElement.Element("bizStep").Value);
        Assert.AreEqual(AssociationEvent.Disposition, eventElement.Element("disposition").Value);
        Assert.AreEqual(AssociationEvent.Epcs.Count(x => x.Type == EpcType.ChildEpc), eventElement.Element("childEPCs").Elements().Count());
        Assert.AreEqual(AssociationEvent.ReadPoint, eventElement.Element("readPoint").Element("id").Value);
        Assert.AreEqual(AssociationEvent.BusinessLocation, eventElement.Element("bizLocation").Element("id").Value);
        Assert.AreEqual(1, eventElement.Element("ilmd").Elements().Count());
    }
}
