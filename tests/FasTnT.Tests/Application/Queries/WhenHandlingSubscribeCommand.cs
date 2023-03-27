﻿using FasTnT.Application.Handlers;
using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Services.Storage;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingSubscribeCommand
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingSubscribeCommand));
    readonly static TestSubscriptionListener Listener = new();

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
        Context.Add(new Subscription
        {
            Name = "TestSubscription",
            QueryName = "SimpleEventQuery",
            Destination = "",
            FormatterName = "TestFormatter"
        });

        Context.SaveChanges();
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
            Trigger = "test"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);
        var result = handler.RegisterSubscriptionAsync(subscription, new TestResultSender(), CancellationToken.None).Result;

        Assert.IsInstanceOfType<Subscription>(result);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameAlreadyExist()
    {
        var subscription = new Subscription
        {
            Name = "TestSubscription",
            Destination = "https://test.com/",
            FormatterName = string.Empty,
            QueryName = "SimpleEventQuery",
            Trigger = "test"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, new TestResultSender(), CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSubscriptionHasEmptyDestination()
    {
        var subscription = new Subscription
        {
            Name = "TestSubscription",
            Destination = "",
            FormatterName = string.Empty,
            QueryName = "SimpleEventQuery",
            Trigger = "test"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, new TestResultSender(), CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSubscriptionHasNoScheduleOrTrigger()
    {
        var subscription = new Subscription
        {
            Name = "TestSubscription",
            Destination = "https://test.com",
            FormatterName = string.Empty,
            QueryName = "SimpleEventQuery"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, new TestResultSender(), CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSpecifiedQueryNameDoesNotExist()
    {
        var subscription = new Subscription
        {
            Name = "InvalidSubscription",
            Destination = "",
            QueryName = "UnknownQuery"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, new TestResultSender(), CancellationToken.None));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfTheSpecifiedQueryDoesNotAllowSubscription()
    {
        var subscription = new Subscription
        {
            Name = "MasterdataTestSubscription",
            Destination = "",
            QueryName = "SimpleMasterdataQuery"
        };
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.RegisterSubscriptionAsync(subscription, new TestResultSender(), CancellationToken.None));
    }
}