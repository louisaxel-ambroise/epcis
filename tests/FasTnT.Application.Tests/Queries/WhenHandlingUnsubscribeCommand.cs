using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingUnsubscribeCommand
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingUnsubscribeCommand));
    readonly static TestSubscriptionListener Listener = new();

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
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);
        handler.DeleteSubscriptionAsync(subscription, CancellationToken.None).Wait();

        Assert.AreEqual(0, Context.Set<Subscription>().Count());
        Assert.IsTrue(Listener.IsRemoved(subscription));
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameDoesNotExist()
    {
        var subscription = "UnknownSubscription";
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.DeleteSubscriptionAsync(subscription, CancellationToken.None));
    }
}
