using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using System.Text.Json;
using System.Text.Json.Nodes;
using FasTnT.Host.Communication.Json.Formatters;
using FasTnT.Host.Endpoints.Interfaces;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.Json;

[TestClass]
public class WhenFormattingAnObjectEvent
{
    public Event ObjectEvent { get; set; }
    public string Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        ObjectEvent = new Event
        {
            Type = EventType.ObjectEvent,
            EventTime = new DateTime(2026, 09, 25, 07, 39, 24),
            EventTimeZoneOffset = +180,
            BusinessStep = "step",
            Disposition = "testDisp",
            BusinessLocation = "loc",
            ReadPoint = "readPointTest",
            EventId = "ni://test",
            Action = EventAction.Add,
            Epcs = [new Epc { Type = EpcType.ParentId, Id = "test:epc:parent" }, new Epc { Type = EpcType.ChildEpc, Id = "test:childepc" }],
            Fields = [new Field { Type = FieldType.Extension, Name = "field", Namespace = "", TextValue = "my extension" }],
            Request = new Domain.Model.Request { RecordTime = DateTime.Now }
        };

        Formatted = JsonResponseFormatter.Format(new QueryResult(new("test", new() { EventList = [ObjectEvent] })));
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
        var json = JsonNode.Parse(Formatted);
        var eventResult = json["epcisBody"]["queryResults"]["resultsBody"]["eventList"].AsArray().First();

        Assert.AreEqual(ObjectEvent.EventTimeZoneOffset.Representation, eventResult["eventTimeZoneOffset"].GetValue<string>());
        Assert.AreEqual(ObjectEvent.EventTime, eventResult["eventTime"].GetValue<DateTime>());
        Assert.AreEqual(ObjectEvent.EventId, eventResult["eventID"].GetValue<string>());
        Assert.AreEqual(ObjectEvent.Action.ToString().ToUpper(), eventResult["action"].GetValue<string>());
        Assert.AreEqual(ObjectEvent.Epcs.Single(x => x.Type == EpcType.ParentId).Id, eventResult["parentID"].GetValue<string>());
        Assert.AreEqual(ObjectEvent.BusinessStep, eventResult["bizStep"].GetValue<string>());
        Assert.AreEqual(ObjectEvent.Disposition, eventResult["disposition"].GetValue<string>());
        Assert.AreEqual(ObjectEvent.Epcs.Count(x => x.Type == EpcType.ChildEpc), eventResult["childEPCs"].AsArray().Count);
        Assert.AreEqual(ObjectEvent.ReadPoint, eventResult["readPoint"]["id"].GetValue<string>());
        Assert.AreEqual(ObjectEvent.BusinessLocation, eventResult["bizLocation"]["id"].GetValue<string>());
        var extension = Assert.ContainsSingle(eventResult["extension"].AsObject());

        Assert.AreEqual("field", extension.Key);
        Assert.AreEqual("my extension", extension.Value.GetValue<string>());
    }
}
