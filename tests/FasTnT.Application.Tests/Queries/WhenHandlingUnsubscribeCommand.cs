using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Notifications;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingUnsubscribeCommand
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingUnsubscribeCommand));
    readonly static EpcisEvents EpcisEvents = new();
    readonly static List<int> RemovedSubscriptions = [];

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
        EpcisEvents.OnUnsubscribe -= RemovedSubscriptions.Add;
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
        EpcisEvents.OnUnsubscribe += RemovedSubscriptions.Add;
    }

    [TestMethod]
    public void ItShouldReturnAnUnubscribeResult()
    {
        var subscription = "TestSubscription";
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), EpcisEvents);
        handler.DeleteSubscriptionAsync(subscription, CancellationToken.None).Wait();

        Assert.AreEqual(0, Context.Set<Subscription>().Count());
        Assert.HasCount(1, RemovedSubscriptions);
    }

    [TestMethod]
    public async Task ItShouldThrowAnExceptionIfASubscriptionWithTheSameNameDoesNotExist()
    {
        var subscription = "UnknownSubscription";
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), EpcisEvents);

        await Assert.ThrowsAsync<EpcisException>(() => handler.DeleteSubscriptionAsync(subscription, CancellationToken.None));
    }
}
