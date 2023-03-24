using FasTnT.Application.Handlers;
using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Services.Storage;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequestGivenAnEventIsInvalid
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequestGivenAnEventIsInvalid));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static TestSubscriptionListener SubscriptionListener = new();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionAnNotCaptureTheRequest()
    {
        var handler = new CaptureHandler(Context, UserContext, SubscriptionListener);
        var request = new Request { SchemaVersion = "1.0", Events = new() { new Event { Type = EventType.AggregationEvent, Action = EventAction.Add } } }; // Does not have parent -> invalid event
        
        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StoreAsync(request, default));
        Assert.AreEqual(0, Context.Set<Request>().Count());
        Assert.IsFalse(SubscriptionListener.IsTriggered("stream"));
    }
}
