using FasTnT.Application.Database;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Subscriptions;

[TestClass]
public class SubscriptionRunnerTests
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(SubscriptionRunnerTests));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly SubscriptionRunner SubscriptionRunner = new SubscriptionRunner(Context);

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.AddRange(new object[] {
            new Request
            {
                CaptureId = "capture_id_test",
                RecordTime =  new DateTime(2020, 03, 15, 21, 14, 10),
                Id = 1,
                SchemaVersion = "2.0",
                Events = new List<Event> {
                    new Event
                    {
                        Type = EventType.ObjectEvent,
                        Action = EventAction.Add,
                        BusinessLocation = "test_location",
                        CorrectiveDeclarationTime = DateTime.UtcNow,
                        CorrectiveReason = "invalid_evt",
                        CorrectiveEventIds = new List<CorrectiveEventId>{{ new CorrectiveEventId { CorrectiveId = "ni://test2" } }},
                        BusinessStep = "step",
                        EventId = "ni://test",
                        EventTime =  new DateTime(2020, 02, 15, 21, 14, 10),
                        EventTimeZoneOffset = "+02:00",
                        Epcs = new List<Epc>{ new Epc { Id = "epc1", Type = EpcType.List } }
                    }
                }
            }
        });

        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    [TestMethod]
    public void ItShouldReturnASuccessfulExecutionResult()
    {
        var context = new SubscriptionContext(Array.Empty<QueryParameter>(), Array.Empty<int>());
        var result = SubscriptionRunner.ExecuteAsync(context, CancellationToken.None).Result;

        Assert.IsTrue(result.Successful);
        Assert.AreEqual(1, result.Events.Count);
        Assert.AreEqual(1, result.RequestIds.Count);
        Assert.IsNull(result.Exception);
    }

    [TestMethod]
    public void ItShouldExcludeTheEventsSpecifiedInTheContext()
    {
        var context = new SubscriptionContext(Array.Empty<QueryParameter>(), new[] { 1 });
        var result = SubscriptionRunner.ExecuteAsync(context, CancellationToken.None).Result;

        Assert.IsTrue(result.Successful);
        Assert.AreEqual(0, result.Events.Count);
        Assert.AreEqual(1, result.RequestIds.Count);
        Assert.IsNull(result.Exception);
    }

    [TestMethod]
    public void ItShouldReturnAnExceptionIfExecutionFails()
    {
        var context = new SubscriptionContext(new[] { QueryParameter.Create("ERROR", "unknown_param") }, Array.Empty<int>());
        var result = SubscriptionRunner.ExecuteAsync(context, CancellationToken.None).Result;

        Assert.IsFalse(result.Successful);
        Assert.IsNull(result.Events);
        Assert.IsNull(result.RequestIds);
        Assert.IsNotNull(result.Exception);
    }
}
