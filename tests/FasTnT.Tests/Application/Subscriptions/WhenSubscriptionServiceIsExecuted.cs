using FasTnT.Application.Services.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FasTnT.Application.Tests.Subscriptions;

[TestClass]
public class WhenSubscriptionServiceIsExecuted
{
    public static SubscriptionService SubscriptionService { get; set; }
    public static SubscriptionContext[] Subscriptions { get; private set; }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        var servicesCollection = new ServiceCollection();
        servicesCollection.AddScoped<ISubscriptionRunner, TestSubscriptionRunner>();
        var providerFactory = new DefaultServiceProviderFactory();
        var provider = providerFactory.CreateServiceProvider(servicesCollection);

        SubscriptionService = new SubscriptionService(provider, new Logger<SubscriptionService>(new NullLoggerFactory()));
        Subscriptions = new SubscriptionContext[]
        {
            new SubscriptionContext(new Domain.Model.Subscriptions.Subscription
            {
                Schedule = new Domain.Model.Subscriptions.SubscriptionSchedule{ Second = "0" },
                ReportIfEmpty = true
            },
            new TestResultSender()),
            new SubscriptionContext(new Domain.Model.Subscriptions.Subscription
            {
                Trigger = "testTrigger",
                ReportIfEmpty = true
            },
            new TestResultSender()),
            new SubscriptionContext(new Domain.Model.Subscriptions.Subscription
            {
                Trigger = "SecondTrigger",
                ReportIfEmpty = true
            },
            new TestResultSender())
        };

        foreach(var subscription in Subscriptions)
        {
            SubscriptionService.Register(subscription);
        }

        SubscriptionService.Trigger(new[] { "SecondTrigger" }).Wait();
        SubscriptionService.Execute(DateTime.UtcNow.AddDays(1), CancellationToken.None);
    }

    [TestMethod]
    public void ItShouldExecuteTheScheduledSubscriptions()
    {
        var triggeredResultSender = Subscriptions[0].ResultSender as TestResultSender;

        Assert.IsTrue(triggeredResultSender.ResultSent);
    }

    [TestMethod]
    public void ItShouldNotExecuteTheSubscriptionsWithoutSchedule()
    {
        var nonTriggeredResultSender = Subscriptions[1].ResultSender as TestResultSender;

        Assert.IsFalse(nonTriggeredResultSender.ResultSent);
    }

    [TestMethod]
    public void ItShouldExecuteTheSubscriptionsTriggered()
    {
        var triggeredResultSender = Subscriptions[2].ResultSender as TestResultSender;

        Assert.IsTrue(triggeredResultSender.ResultSent);
    }
}
