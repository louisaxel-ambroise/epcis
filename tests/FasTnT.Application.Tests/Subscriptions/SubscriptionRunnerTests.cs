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
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(SubscriptionRunnerTests));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly SubscriptionRunner SubscriptionRunner = new(Context.DbContext);

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.DbContext.AddRange([
            new Request
            {
                CaptureId = "capture_id_test",
                RecordTime =  new DateTime(2020, 03, 15, 21, 14, 10),
                Id = 1,
                SchemaVersion = "2.0",
                Events = [
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
                        Epcs = [new Epc { Id = "epc1", Type = EpcType.List }]
                    }
                ]
            }
        ]);

        Context.DbContext.SaveChanges();
        Context.DbContext.ChangeTracker.Clear();
    }

    [TestMethod]
    public void ItShouldReturnASuccessfulExecutionResult()
    {
        var context = new SubscriptionContext([], []);
        var result = SubscriptionRunner.ExecuteAsync(context, CancellationToken.None).Result;

        Assert.IsTrue(result.Successful);
        Assert.HasCount(1, result.Events);
        Assert.HasCount(1, result.RequestIds);
        Assert.IsNull(result.Exception);
    }

    [TestMethod]
    public void ItShouldExcludeTheEventsSpecifiedInTheContext()
    {
        var context = new SubscriptionContext([], [1]);
        var result = SubscriptionRunner.ExecuteAsync(context, CancellationToken.None).Result;

        Assert.IsTrue(result.Successful);
        Assert.IsEmpty(result.Events);
        Assert.HasCount(1, result.RequestIds);
        Assert.IsNull(result.Exception);
    }

    [TestMethod]
    public void ItShouldReturnAnExceptionIfExecutionFails()
    {
        var context = new SubscriptionContext([QueryParameter.Create("ERROR", "unknown_param")], []);
        var result = SubscriptionRunner.ExecuteAsync(context, CancellationToken.None).Result;

        Assert.IsFalse(result.Successful);
        Assert.IsNull(result.Events);
        Assert.IsNull(result.RequestIds);
        Assert.IsNotNull(result.Exception);
    }
}
