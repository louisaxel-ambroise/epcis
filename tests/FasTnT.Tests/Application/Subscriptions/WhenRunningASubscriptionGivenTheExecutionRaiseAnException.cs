using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Services.Subscriptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FasTnT.Application.Tests.Subscriptions;

[TestClass]
public class WhenRunningASubscriptionGivenTheExecutionRaiseAnException
{
    public static ISubscriptionRunner SubscriptionRunner { get; set; }
    public static EpcisContext Context { get; set; }
    public static Subscription Subscription { get; set; }
    public static TestResultSender ResultSender { get; set; }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context = EpcisTestContext.GetContext(nameof(WhenRunningASubscriptionGivenNoPendingEventAndNoReportIfEmpty));
        SubscriptionRunner = new SubscriptionRunner(Context, new Logger<SubscriptionRunner>(new NullLoggerFactory()));
        Subscription = new Subscription { Destination = "test", QueryName = "SimpleEventQuery", Parameters = new List<QueryParameter> { new QueryParameter { Name = "Unknown", Values = new[] { "" }  } }, Name = "test_subscription", FormatterName = "" };
        ResultSender = new TestResultSender();

        Context.Add(Subscription);
        Context.SaveChanges();

        Context.Add(new Request
        {
            Id = 1,
            CaptureId = "1",
            SchemaVersion = "2.0",
            CaptureTime = DateTime.UtcNow,
            Events = new List<Event> { new Event { Type = EventType.ObjectEvent, EventTime = DateTime.UtcNow } }
        });
        Context.SaveChanges();

        SubscriptionRunner.RunAsync(new SubscriptionContext(Subscription, ResultSender), DateTime.UtcNow, CancellationToken.None).Wait();
    }

    [TestMethod]
    public void ItShouldNotSendTheResults()
    {
        Assert.IsFalse(ResultSender.ResultSent);
    }

    [TestMethod]
    public void ItShouldSendAnError()
    {
        Assert.IsTrue(ResultSender.ErrorSent);
    }

    [TestMethod]
    public void ItShouldRegisterASubscriptionExecutionRecord()
    {
        Assert.AreEqual(1, Context.Set<SubscriptionExecutionRecord>().Count());
    }

    [TestMethod]
    public void ItShouldRemoveThePendingRequests()
    {
        Assert.AreEqual(0, Context.Set<PendingRequest>().Count());
    }
}
