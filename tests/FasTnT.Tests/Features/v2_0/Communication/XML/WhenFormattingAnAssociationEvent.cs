using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication.Formatters;
using FasTnT.Host.Features.v2_0.Communication.Utils;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

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
            Fields = new List<Field> { new Field { Type = FieldType.Ilmd, Name = "field", Namespace = "fastnt.ns", TextValue = "OK"} }
        };

        Formatted = XmlEventFormatter.FormatList(new List<Event> { AssociationEvent }).FirstOrDefault();
    }

    [TestMethod]
    public void ItShouldFormatTheEventToXml()
    {
        Assert.IsNotNull(Formatted);
        Assert.AreEqual("AssociationEvent", Formatted.Name.LocalName);
    }

    [TestMethod]
    public void ItShouldFormatTheEventCorrectly()
    {
        Assert.AreEqual(AssociationEvent.EventTimeZoneOffset.Representation, Formatted.Element("eventTimeZoneOffset").Value);
        Assert.AreEqual(AssociationEvent.EventId, Formatted.Element("eventID").Value);
        Assert.AreEqual(AssociationEvent.Action.ToUpperString(), Formatted.Element("action").Value);
        Assert.AreEqual(AssociationEvent.Epcs.Single(x => x.Type == EpcType.ParentId).Id, Formatted.Element("parentID").Value);
        Assert.AreEqual(AssociationEvent.BusinessStep, Formatted.Element("bizStep").Value);
        Assert.AreEqual(AssociationEvent.Disposition, Formatted.Element("disposition").Value);
        Assert.AreEqual(AssociationEvent.Epcs.Count(x => x.Type == EpcType.ChildEpc), Formatted.Element("childEPCs").Elements().Count());
        Assert.AreEqual(AssociationEvent.ReadPoint, Formatted.Element("readPoint").Element("id").Value);
        Assert.AreEqual(AssociationEvent.BusinessLocation, Formatted.Element("bizLocation").Element("id").Value);
        Assert.AreEqual(1, Formatted.Element("ilmd").Elements().Count());
    }
}
