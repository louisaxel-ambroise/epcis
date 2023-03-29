using FasTnT.Application.Handlers;
using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application;
using FasTnT.Tests.Application.Context;

namespace FasTnT.Tests.Application.Capture;

[TestClass]
public class WhenHandlingCaptureRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static List<Request> RequestCaptured = new();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
        EpcisEvents.OnRequestCaptured -= RequestCaptured.Add;
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        EpcisEvents.OnRequestCaptured += RequestCaptured.Add;
    }

    [TestMethod]
    public void ItShouldReturnACaptureResultAndStoreTheRequest()
    {
        var handler = new CaptureHandler(Context, UserContext);
        var request = new Request { SchemaVersion = "1.0", Events = new() { new Event { Type = EventType.ObjectEvent } } };
        var result = handler.StoreAsync(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, Context.Set<Request>().Count());
        Assert.AreEqual(1, RequestCaptured.Count);
    }
}
