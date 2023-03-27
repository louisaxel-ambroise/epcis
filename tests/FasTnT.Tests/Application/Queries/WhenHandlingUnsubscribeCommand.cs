using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Handlers;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingUnsubscribeCommand
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingUnsubscribeCommand));

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
        Context.ChangeTracker.Clear();
    }

    [TestMethod]
    public void ItShouldReturnAnUnubscribeResult()
    {
        var subscription = "TestSubscription";
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser());
        handler.DeleteSubscriptionAsync(subscription, CancellationToken.None).Wait();

        Assert.AreEqual(0, Context.Set<Subscription>().Count());
        // TODO: Assert.IsTrue(Listener.IsRemoved(subscription));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameDoesNotExist()
    {
        var subscription = "UnknownSubscription";
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser());

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.DeleteSubscriptionAsync(subscription, CancellationToken.None));
    }
}
