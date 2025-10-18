using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Notifications;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingSubscribeCommand
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingSubscribeCommand));

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Add(new Subscription
        {
            Name = "TestSubscription",
            QueryName = "SimpleEventQuery",
            Destination = "",
            FormatterName = "TestFormatter"
        });

        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    [TestMethod]
    public void ItShouldRegisterTheSubscriptionIfCreatedSuccessfully()
    {
        var subscription = new Subscription
        {
            Name = "NewSubscription",
            Destination = "https://test.com/",
            FormatterName = string.Empty,
            QueryName = "SimpleEventQuery",
            Trigger = "daily"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), new EpcisEvents());
        var result = handler.RegisterSubscriptionAsync(subscription, CancellationToken.None).Result;

        Assert.IsInstanceOfType<Subscription>(result);
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameAlreadyExist()
    {
        var subscription = new Subscription
        {
            Name = "TestSubscription",
            Destination = "https://test.com/",
            FormatterName = string.Empty,
            QueryName = "SimpleEventQuery",
            Trigger = "daily"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), new EpcisEvents());

        await Assert.ThrowsAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheSubscriptionHasEmptyDestination()
    {
        var subscription = new Subscription
        {
            Name = "TestSubscription",
            Destination = "",
            FormatterName = string.Empty,
            QueryName = "SimpleEventQuery",
            Trigger = "daily"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), new EpcisEvents());

        await Assert.ThrowsAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheTriggerIsInvalid()
    {
        var subscription = new Subscription
        {
            Name = "TestSubscription",
            Destination = "",
            FormatterName = string.Empty,
            QueryName = "SimpleEventQuery",
            Trigger = "unknown"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), new EpcisEvents());

        await Assert.ThrowsAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheSubscriptionHasNoScheduleOrTrigger()
    {
        var subscription = new Subscription
        {
            Name = "TestSubscription",
            Destination = "https://test.com",
            FormatterName = string.Empty,
            QueryName = "SimpleEventQuery"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), new EpcisEvents());

        await Assert.ThrowsAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheSpecifiedQueryNameDoesNotExist()
    {
        var subscription = new Subscription
        {
            Name = "InvalidSubscription",
            Destination = "",
            QueryName = "UnknownQuery"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), new EpcisEvents());

        await Assert.ThrowsAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, CancellationToken.None));
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfTheSpecifiedQueryDoesNotAllowSubscription()
    {
        var subscription = new Subscription
        {
            Name = "MasterdataTestSubscription",
            Destination = "",
            QueryName = "SimpleMasterdataQuery"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), new EpcisEvents());

        await Assert.ThrowsAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, CancellationToken.None));
    }
}
