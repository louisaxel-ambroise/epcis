using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.Json;

[TestClass]
public class WhenFormattingAnAssociationEvent
{
    public Event AssociationEvent { get; set; }
    public string Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        AssociationEvent = new Event
        {
            Type = EventType.AssociationEvent,
            EventTime = new DateTime(2021, 01, 24, 13, 30, 24),
            EventTimeZoneOffset = -180,
            BusinessStep = "step",
            Disposition = "testDisp",
            BusinessLocation = "loc",
            ReadPoint = "readPointTest",
            EventId = "ni://test",
            Action = EventAction.Add,
            Epcs = [new Epc { Type = EpcType.ParentId, Id = "test:epc:parent" }, new Epc { Type = EpcType.ChildEpc, Id = "test:childepc" }],
            Fields = [new Field { Type = FieldType.Ilmd, Name = "field", Namespace = "fastnt.ns", TextValue = "OK" }],
            Request = new Domain.Model.Request { RecordTime = DateTime.Now }
        };

        Formatted = JsonResponseFormatter.Format(new QueryResult(new ("test", new() { EventList = [AssociationEvent] })));
    }

    [TestMethod]
    public void ItShouldReturnAValidJson()
    {
        Assert.IsNotNull(Formatted);
        Assert.IsNotNull(JsonSerializer.Deserialize<object>(Formatted));
    }

    [TestMethod]
    public void TheJsonShouldContainASingleEvent()
    {
        var json = JsonObject.Parse(Formatted);
        var eventList = json["epcisBody"]["queryResults"]["resultsBody"]["eventList"];

        Assert.IsNotNull(eventList);
        Assert.IsNotNull(eventList.AsArray());
        Assert.AreEqual(1, eventList.AsArray().Count);
    }

    [TestMethod]
    public void TheEventShouldBeCorrectlyFormatted()
    {
        var json = JsonObject.Parse(Formatted);
        var eventResult = json["epcisBody"]["queryResults"]["resultsBody"]["eventList"].AsArray().First();

        Assert.AreEqual(AssociationEvent.EventTimeZoneOffset.Representation, eventResult["eventTimeZoneOffset"].GetValue<string>());
        Assert.AreEqual(AssociationEvent.EventTime, eventResult["eventTime"].GetValue<DateTime>());
        Assert.AreEqual(AssociationEvent.EventId, eventResult["eventID"].GetValue<string>());
        Assert.AreEqual(AssociationEvent.Action.ToString().ToUpper(), eventResult["action"].GetValue<string>());
        Assert.AreEqual(AssociationEvent.Epcs.Single(x => x.Type == EpcType.ParentId).Id, eventResult["parentID"].GetValue<string>());
        Assert.AreEqual(AssociationEvent.BusinessStep, eventResult["bizStep"].GetValue<string>());
        Assert.AreEqual(AssociationEvent.Disposition, eventResult["disposition"].GetValue<string>());
        Assert.AreEqual(AssociationEvent.Epcs.Count(x => x.Type == EpcType.ChildEpc), eventResult["childEPCs"].AsArray().Count);
        Assert.AreEqual(AssociationEvent.ReadPoint, eventResult["readPoint"]["id"].GetValue<string>());
        Assert.AreEqual(AssociationEvent.BusinessLocation, eventResult["bizLocation"]["id"].GetValue<string>());
        Assert.AreEqual(1, eventResult["ilmd"].AsObject().Count);
    }

    [TestMethod]
    public void TheDateShouldBeInUTC()
    {
        var json = JsonObject.Parse(Formatted);
        var eventTime = json["epcisBody"]["queryResults"]["resultsBody"]["eventList"].AsArray().First()["eventTime"].GetValue<string>();

        Assert.AreEqual("Z", eventTime[^1..]);
    }
}
