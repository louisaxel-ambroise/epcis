using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Notifications;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using Microsoft.Extensions.Options;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static EpcisEvents EpcisEvents = new();
    readonly static List<int> CapturedRequests = [];

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
        EpcisEvents.OnCapture -= CapturedRequests.Add;
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        EpcisEvents.OnCapture += CapturedRequests.Add;
    }

    [TestMethod]
    public void ItShouldReturnACaptureResultAndStoreTheRequest()
    {
        var handler = new CaptureHandler(Context, UserContext, EpcisEvents, Options.Create(new Constants()));
        var request = new Request { SchemaVersion = "1.0", Events = [new Event { Type = EventType.ObjectEvent }] };
        var result = handler.StoreAsync(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, Context.Set<Request>().Count());
        Assert.HasCount(1, CapturedRequests);
    }
}
