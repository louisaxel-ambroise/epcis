using FasTnT.Application.Database;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries.Parameters;

[TestClass]
public class WhenApplyingLT_eventTimeFilter
{
    public EpcisContext Context { get; set; }
    public QueryParameter Parameter { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Context = Tests.Context.EpcisTestContext.GetContext("simpleEventQuery");
        Context.Add(new Domain.Model.Request
        {
            Events = new[] {
                new Event
                {
                    Type = Domain.Enumerations.EventType.ObjectEvent,
                    EventTime = new (2021, 01, 12, 10, 24, 10),
                    Action = Domain.Enumerations.EventAction.Observe
                },
                new Event
                {
                    Type = Domain.Enumerations.EventType.TransactionEvent,
                    EventTime = new (2021, 01, 12, 10, 30, 00),
                    Action = Domain.Enumerations.EventAction.Observe
                },
                new Event
                {
                    Type = Domain.Enumerations.EventType.TransactionEvent,
                    EventTime = new (2011, 08, 02, 21, 50, 00),
                    Action = Domain.Enumerations.EventAction.Observe
                }
            }.ToList(),
            CaptureTime = DateTime.Now,
            DocumentTime = DateTime.Now,
            SchemaVersion = "1.2"
        });
        Context.SaveChanges();

        Parameter = QueryParameter.Create("LT_eventTime", new[] { "2020-01-12T10:24:10.000Z" });
    }

    [TestMethod]
    public void ItShouldOnlyReturnTheEventsCaptureBeforeTheDate()
    {
        var result = Context.QueryEvents(new[] { Parameter }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.EventTime < new DateTime(2021, 01, 12, 10, 24, 10)));
    }
}
