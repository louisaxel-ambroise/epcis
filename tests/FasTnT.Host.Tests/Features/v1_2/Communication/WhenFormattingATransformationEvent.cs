using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v1_2.Communication.Formatters;

using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication.XML;

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
            ReadPoint = "readPointTest",
            EventId = "ni://test",
            Action = EventAction.Add,
            Epcs = new List<Epc> { new Epc { Type = EpcType.List, Id = "test:epc" } },
            Transactions = new List<BusinessTransaction> { new BusinessTransaction { Id = "tx", Type = "txtype" } }
        };

        Formatted = XmlEventFormatter.FormatList(new List<Event> { TransformationEvent }).FirstOrDefault();
    }

    [TestMethod]
    public void ItShouldFormatTheEventToXml()
    {
        Assert.IsNotNull(Formatted);
        Assert.AreEqual("extension", Formatted.Name.LocalName);
        Assert.AreEqual("TransformationEvent", Formatted.Elements().First().Name.LocalName);
    }

    [TestMethod]
    public void ItShouldFormatTheEventCorrectly()
    {
        var eventElt = Formatted.Elements().First();

        Assert.AreEqual(TransformationEvent.EventTimeZoneOffset.Representation, eventElt.Element("eventTimeZoneOffset").Value);
        Assert.AreEqual(TransformationEvent.EventId, eventElt.Element("baseExtension").Element("eventID").Value);
        Assert.AreEqual(TransformationEvent.BusinessStep, eventElt.Element("bizStep").Value);
        Assert.AreEqual(TransformationEvent.Transactions.Count, eventElt.Element("bizTransactionList").Elements().Count());
        Assert.AreEqual(TransformationEvent.Disposition, eventElt.Element("disposition").Value);
        Assert.AreEqual(TransformationEvent.BusinessLocation, eventElt.Element("bizLocation").Element("id").Value);
    }
}
