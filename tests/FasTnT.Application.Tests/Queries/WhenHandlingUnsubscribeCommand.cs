using FasTnT.Application.Handlers;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingUnsubscribeCommand
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingUnsubscribeCommand));
    readonly static List<int> RemovedSubscriptions = [];

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
        Context?.Events.OnUnsubscribe -= RemovedSubscriptions.Add;
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.DbContext.Add(new Subscription
        {
            Name = "TestSubscription",
            QueryName = "SimpleEventQuery",
            Destination = "",
            FormatterName = "TestFormatter"
        });

        Context.DbContext.SaveChanges();
        Context.DbContext.ChangeTracker.Clear();
        Context.Events.OnUnsubscribe += RemovedSubscriptions.Add;
    }

    [TestMethod]
    public void ItShouldReturnAnUnubscribeResult()
    {
        var subscription = "TestSubscription";
        var handler = new SubscriptionsHandler(Context.DbContext, new TestCurrentUser());
        handler.DeleteSubscriptionAsync(subscription, CancellationToken.None).Wait(CancellationToken.None);

        Assert.AreEqual(0, Context.DbContext.Set<Subscription>().Count());
        Assert.HasCount(1, RemovedSubscriptions);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameDoesNotExist()
    {
        var subscription = "UnknownSubscription";
        var handler = new SubscriptionsHandler(Context.DbContext, new TestCurrentUser());

        Assert.ThrowsAsync<EpcisException>(() => handler.DeleteSubscriptionAsync(subscription, CancellationToken.None));
    }
}
