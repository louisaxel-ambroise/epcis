using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequest
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static List<int> CapturedRequests = [];

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
        Context.Events.OnCapture -= CapturedRequests.Add;
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Events.OnCapture += CapturedRequests.Add;
    }

    [TestMethod]
    public async Task ItShouldReturnACaptureResultAndStoreTheRequest()
    {
        var handler = new CaptureHandler(Context.DbContext, UserContext, Options.Create(new Constants()));
        var request = new Request { SchemaVersion = "1.0", Events = [new Event { Type = EventType.ObjectEvent }] };
        var result = await handler.StoreAsync(request, default);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, Context.DbContext.Set<Request>().Count());
        Assert.HasCount(1, CapturedRequests);
    }
}
