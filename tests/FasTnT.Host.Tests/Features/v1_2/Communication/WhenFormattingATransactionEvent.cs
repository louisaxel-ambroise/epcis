﻿using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

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
            Epcs = [new Epc { Type = EpcType.List, Id = "test:epc" }],
            Sources = [new Source { Id = "Party", Type = "PartyType" }],
            Destinations = [new Destination { Id = "Dest", Type = "PartyType" }],
            Transactions = [new BusinessTransaction { Id = "tx", Type = "txtype" }],
            Request = new Domain.Model.Request { RecordTime = DateTime.Now }
        };

        Formatted = XmlV1EventFormatter.Instance.FormatList([TransactionEvent]).FirstOrDefault();
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
        Assert.AreEqual(TransactionEvent.EventId, Formatted.Element("baseExtension").Element("eventID").Value);
        Assert.AreEqual(TransactionEvent.Action.ToString().ToUpper(), Formatted.Element("action").Value);
        Assert.AreEqual(TransactionEvent.BusinessStep, Formatted.Element("bizStep").Value);
        Assert.AreEqual(TransactionEvent.Transactions.Count, Formatted.Element("bizTransactionList").Elements().Count());
        Assert.AreEqual(TransactionEvent.Disposition, Formatted.Element("disposition").Value);
        Assert.AreEqual(TransactionEvent.Sources.Count, Formatted.Element("extension").Element("sourceList").Elements().Count());
        Assert.AreEqual(TransactionEvent.Destinations.Count, Formatted.Element("extension").Element("destinationList").Elements().Count());
        Assert.AreEqual(TransactionEvent.Epcs.Count(x => x.Type == EpcType.List), Formatted.Element("epcList").Elements().Count());
        Assert.AreEqual(TransactionEvent.ReadPoint, Formatted.Element("readPoint").Element("id").Value);
        Assert.AreEqual(TransactionEvent.BusinessLocation, Formatted.Element("bizLocation").Element("id").Value);
    }
}
