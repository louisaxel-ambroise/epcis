using FasTnT.Application.Database;
using FasTnT.Application.Events;
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
public class WhenHandlingCaptureRequestGivenAnEventIsInvalid
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequestGivenAnEventIsInvalid));
    readonly static EpcisEvents EpcisEvents = new();
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static List<int> CapturedRequests = [];

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
        EpcisEvents.OnRequestCaptured -= CapturedRequests.Add;
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        EpcisEvents.OnRequestCaptured += CapturedRequests.Add;
    }

    [TestMethod]
    public void ItShouldThrowAnExceptionAnNotCaptureTheRequest()
    {
        var handler = new CaptureHandler(Context, UserContext, EpcisEvents, Options.Create(new Constants()));
        var request = new Request { SchemaVersion = "1.0", Events = [new Event { Type = EventType.AggregationEvent, Action = EventAction.Add }] }; // Does not have parent -> invalid event

        Assert.ThrowsExceptionAsync<EpcisException>(() => handler.StoreAsync(request, default));
        Assert.AreEqual(0, Context.Set<Request>().Count());
        Assert.AreEqual(0, CapturedRequests.Count);
    }
}
