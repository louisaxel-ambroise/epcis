using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingATransformationEvent
{
    public Event TransformationEvent { get; set; }
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        TransformationEvent = new Event
        {
            Type = EventType.TransformationEvent,
            EventTimeZoneOffset = "+02:00",
            BusinessStep = "step",
            Disposition = "testDisp",
            BusinessLocation = "loc",
            CorrectiveDeclarationTime = DateTime.UtcNow,
            CorrectiveReason = "invalid events",
            CorrectiveEventIds = new List<CorrectiveEventId> { new CorrectiveEventId { CorrectiveId = "ni://prev-evt" } },
            ReadPoint = "readPointTest",
            EventId = "ni://test",
            Epcs = new List<Epc> { new Epc { Type = EpcType.List, Id = "test:epc" } },
            Transactions = new List<BusinessTransaction> { new BusinessTransaction { Id = "tx", Type = "txtype" } },
            Request = new Domain.Model.Request { RecordTime = DateTime.Now }
        };

        Formatted = XmlEventFormatter.FormatList(new List<Event> { TransformationEvent }).FirstOrDefault();
    }

    [TestMethod]
    public void ItShouldFormatTheEventToXml()
    {
        Assert.IsNotNull(Formatted);
        Assert.AreEqual("TransformationEvent", Formatted.Name.LocalName);
    }

    [TestMethod]
    public void ItShouldFormatTheEventCorrectly()
    {
        Assert.AreEqual(TransformationEvent.EventTimeZoneOffset.Representation, Formatted.Element("eventTimeZoneOffset").Value);
        Assert.AreEqual(TransformationEvent.EventId, Formatted.Element("eventID").Value);
        Assert.AreEqual(TransformationEvent.CorrectiveReason, Formatted.Element("errorDeclaration").Element("reason").Value);
        Assert.AreEqual(TransformationEvent.CorrectiveEventIds.Count, Formatted.Element("errorDeclaration").Element("correctiveEventIDs").Elements().Count());
        Assert.AreEqual(TransformationEvent.BusinessStep, Formatted.Element("bizStep").Value);
        Assert.AreEqual(TransformationEvent.Transactions.Count, Formatted.Element("bizTransactionList").Elements().Count());
        Assert.AreEqual(TransformationEvent.Disposition, Formatted.Element("disposition").Value);
        Assert.AreEqual(TransformationEvent.ReadPoint, Formatted.Element("readPoint").Element("id").Value);
        Assert.AreEqual(TransformationEvent.BusinessLocation, Formatted.Element("bizLocation").Element("id").Value);
    }
}
