using FasTnT.Application;
using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Handlers;
using FasTnT.Tests.Application.Context;

namespace FasTnT.Tests.Application.Queries;

[TestClass]
public class WhenHandlingUnsubscribeCommand
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingUnsubscribeCommand));
    readonly static List<Subscription> RemovedSubscriptions = new();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
        EpcisEvents.OnSubscriptionRemoved -= RemovedSubscriptions.Add;
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
        EpcisEvents.OnSubscriptionRemoved += RemovedSubscriptions.Add;
    }

    [TestMethod]
    public void ItShouldReturnAnUnubscribeResult()
    {
        var subscription = "TestSubscription";
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser());
        handler.DeleteSubscriptionAsync(subscription, CancellationToken.None).Wait();

        Assert.AreEqual(0, Context.Set<Subscription>().Count());
        Assert.AreEqual(1, RemovedSubscriptions.Count);
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameDoesNotExist()
    {
        var subscription = "UnknownSubscription";
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser());

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.DeleteSubscriptionAsync(subscription, CancellationToken.None));
    }
}
