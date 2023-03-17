using FasTnT.Application.Database;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries.Parameters;

[TestClass]
public class WhenApplyingeventTypeFilter
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
                    Action = Domain.Enumerations.EventAction.Observe
                },
                new Event
                {
                    Type = Domain.Enumerations.EventType.TransactionEvent,
                    Action = Domain.Enumerations.EventAction.Observe
                }
            }.ToList(),
            CaptureTime = DateTime.Now,
            DocumentTime = DateTime.Now,
            SchemaVersion = "1.2"
        });
        Context.SaveChanges();

        Parameter = QueryParameter.Create("eventType", "ObjectEvent");
    }

    [TestMethod]
    public void ItShouldOnlyReturnTheEventsOfTheSpecifiedType()
    {
        var result = Context.QueryEvents(new[] { Parameter }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Type == Domain.Enumerations.EventType.ObjectEvent));
    }
}
