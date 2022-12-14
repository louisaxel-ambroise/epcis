using FasTnT.Application.Database;
using FasTnT.Application.Services.DataSources;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries.Parameters;

[TestClass]
public class WhenApplyingGE_eventTimeFilter
{
    public EpcisContext Context { get; set; }
    public IEpcisDataSource Query { get; set; }
    public IList<QueryParameter> Parameters { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Context = Tests.Context.EpcisTestContext.GetContext("simpleEventQuery");
        Query = new EventDataSource(Context);

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

        Parameters = new[] { QueryParameter.Create("GE_eventTime", new[] { "2020-01-12T10:24:10.000Z" }) }.ToList();
    }

    [TestMethod]
    public void ItShouldOnlyReturnTheEventsCaptureAfterOrOnTheDate()
    {
        Query.ApplyParameters(Parameters);
        var result = Query.ExecuteAsync( default).Result;
        Assert.AreEqual(2, result.EventList.Count);
        Assert.IsTrue(result.EventList.All(x => x.EventTime >= new DateTime(2021, 01, 12, 10, 24, 10)));
    }
}
