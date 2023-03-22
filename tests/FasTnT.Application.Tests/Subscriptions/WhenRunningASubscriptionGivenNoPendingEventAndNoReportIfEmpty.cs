using FasTnT.Application.Database;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FasTnT.Application.Tests.Subscriptions;

[TestClass]
public class WhenRunningASubscriptionGivenNoPendingEventAndNoReportIfEmpty
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
        Subscription = new Subscription { Destination = "test", ReportIfEmpty = false, QueryName = "SimpleEventQuery", Name = "test_subscription", FormatterName = "XmlResultSender" };
        ResultSender = new TestResultSender();

        Context.Add(Subscription);
        Context.SaveChanges();

        SubscriptionRunner.RunAsync(new SubscriptionContext(Subscription, ResultSender), DateTime.UtcNow, CancellationToken.None).Wait();
    }

    [TestMethod]
    public void ItShouldNotSendTheResults()
    {
        Assert.IsFalse(ResultSender.ResultSent);
    }

    [TestMethod]
    public void ItShouldNotSendAnError()
    {
        Assert.IsFalse(ResultSender.ErrorSent);
    }

    [TestMethod]
    public void ItShouldRegisterASubscriptionExecutionRecord()
    {
        Assert.AreEqual(1, Context.Set<SubscriptionExecutionRecord>().Count());
    }

    [TestMethod]
    public void TheSubscriptionExecutionRecordShouldBeSuccessful()
    {
        Assert.IsTrue(Context.Set<SubscriptionExecutionRecord>().First().Successful);
    }
}
