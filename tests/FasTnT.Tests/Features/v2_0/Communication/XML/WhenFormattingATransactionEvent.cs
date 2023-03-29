﻿using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication.Formatters;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingATransactionEvent
{
    public Event TransactionEvent { get; set; }
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        TransactionEvent = new Event
        {
            Type = EventType.TransactionEvent,
            EventTimeZoneOffset = "+02:00",
            BusinessStep = "step",
            Disposition = "testDisp",
            BusinessLocation = "loc",
            ReadPoint = "readPointTest",
            EventId = "ni://test",
            Action = EventAction.Add,
            Sources = new List<Source> { new Source { Id = "Party", Type = "PartyType" } },
            Destinations = new List<Destination> { new Destination { Id = "Dest", Type = "PartyType" } },
            Epcs = new List<Epc> { new Epc { Type = EpcType.List, Id = "test:epc" } },
            Transactions = new List<BusinessTransaction> { new BusinessTransaction { Id = "tx", Type = "txtype" } }
        };

        Formatted = XmlEventFormatter.FormatList(new List<Event> { TransactionEvent }).FirstOrDefault();
    }

    [TestMethod]
    public void ItShouldFormatTheEventToXml()
    {
        Assert.IsNotNull(Formatted);
        Assert.AreEqual("TransactionEvent", Formatted.Name.LocalName);
    }

    [TestMethod]
    public void ItShouldFormatTheEventCorrectly()
    {
        Assert.AreEqual(TransactionEvent.EventTimeZoneOffset.Representation, Formatted.Element("eventTimeZoneOffset").Value);
        Assert.AreEqual(TransactionEvent.EventId, Formatted.Element("eventID").Value);
        Assert.AreEqual(TransactionEvent.Action.ToUpperString(), Formatted.Element("action").Value);
        Assert.AreEqual(TransactionEvent.BusinessStep, Formatted.Element("bizStep").Value);
        Assert.AreEqual(TransactionEvent.Transactions.Count, Formatted.Element("bizTransactionList").Elements().Count());
        Assert.AreEqual(TransactionEvent.Sources.Count, Formatted.Element("sourceList").Elements().Count());
        Assert.AreEqual(TransactionEvent.Destinations.Count, Formatted.Element("destinationList").Elements().Count());
        Assert.AreEqual(TransactionEvent.Disposition, Formatted.Element("disposition").Value);
        Assert.AreEqual(TransactionEvent.Epcs.Count(x => x.Type == EpcType.List), Formatted.Element("epcList").Elements().Count());
        Assert.AreEqual(TransactionEvent.ReadPoint, Formatted.Element("readPoint").Element("id").Value);
        Assert.AreEqual(TransactionEvent.BusinessLocation, Formatted.Element("bizLocation").Element("id").Value);
    }
}
