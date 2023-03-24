using FasTnT.Application.Handlers;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingTriggerCommand
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingTriggerCommand));
    readonly static TestSubscriptionListener Listener = new();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [TestMethod]
    public void ItShouldTriggerTheListener()
    {
        var handler = new SubscriptionsHandler(Context, new TestCurrentUser(), Listener);
        handler.TriggerSubscriptionAsync(new[] { "Trigger1", "Trigger2" }, CancellationToken.None).Wait();

        Assert.IsTrue(Listener.IsTriggered("Trigger1"));
        Assert.IsTrue(Listener.IsTriggered("Trigger2"));
        Assert.IsFalse(Listener.IsTriggered("Trigger3"));
    }
}
