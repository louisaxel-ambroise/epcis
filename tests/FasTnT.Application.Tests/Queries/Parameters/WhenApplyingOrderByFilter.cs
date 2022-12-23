using FasTnT.Application.Database;
using FasTnT.Application.UseCases.DataSources.Contexts;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries.Parameters;

[TestClass]
public class WhenApplyingOrderByFilter
{
    public EpcisContext Context { get; set; }
    public EventQueryContext Query { get; set; }
    public QueryParameter Parameter { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Context = Tests.Context.EpcisTestContext.GetContext(nameof(WhenApplyingOrderByFilter));
        Query = new EventQueryContext(Context);

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

        Parameter = QueryParameter.Create("orderBy", new[] { "eventTime" });
    }

    [TestMethod]
    public void ItShouldReturnAllTheEventsWithTheCorrectOrder()
    {
        Query.Parse(new[] { Parameter });
        var result = Query.Apply(Context.Set<Event>()).ToList();
        var sorted = result.OrderByDescending(s => s.EventTime);

        Assert.AreEqual(3, result.Count);
        CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
    }
}
