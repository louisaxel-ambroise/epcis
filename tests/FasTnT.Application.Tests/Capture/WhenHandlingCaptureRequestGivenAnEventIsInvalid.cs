using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Notifications;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequestGivenAnEventIsInvalid
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequestGivenAnEventIsInvalid));
    readonly static EpcisEvents EpcisEvents = new();
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static List<int> CapturedRequests = [];

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
        EpcisEvents.OnCapture -= CapturedRequests.Add;
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        EpcisEvents.OnCapture += CapturedRequests.Add;
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionAnNotCaptureTheRequest()
    {
        var handler = new CaptureHandler(Context.DbContext, UserContext, default);
        var request = new Request { SchemaVersion = "1.0", Events = [new Event { Type = EventType.AggregationEvent, Action = EventAction.Add }] }; // Does not have parent -> invalid event

        Assert.ThrowsAsync<EpcisException>(() => handler.StoreAsync(request, default));
        Assert.AreEqual(0, Context.DbContext.Set<Request>().Count());
        Assert.IsEmpty(CapturedRequests);
    }
}
