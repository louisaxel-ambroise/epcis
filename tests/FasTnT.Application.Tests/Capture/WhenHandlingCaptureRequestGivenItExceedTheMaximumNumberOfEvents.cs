using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using Microsoft.Extensions.Options;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequestGivenItExceedTheMaximumNumberOfEvents
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequestGivenItExceedTheMaximumNumberOfEvents));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static List<int> CapturedRequests = [];
    static Constants Constants = new();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
        Context?.Events.OnCapture -= CapturedRequests.Add;
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Constants = new Constants() { MaxEventsCapturePerCall = 1 };
        Context?.Events.OnCapture += CapturedRequests.Add;
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionAnNotCaptureTheRequest()
    {
        var handler = new CaptureHandler(Context.DbContext, UserContext, Options.Create(Constants));
        var request = new Request { SchemaVersion = "1.0", Events = [new Event { Type = EventType.ObjectEvent }, new Event { Type = EventType.ObjectEvent }] };

        Assert.ThrowsAsync<EpcisException>(() => handler.StoreAsync(request, default));
        Assert.AreEqual(0, Context.DbContext.Set<Request>().Count());
        Assert.IsEmpty(CapturedRequests);
    }
}
