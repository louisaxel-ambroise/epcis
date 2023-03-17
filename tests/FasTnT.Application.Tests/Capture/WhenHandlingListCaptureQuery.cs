using FasTnT.Application.Database;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.Handlers;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Enumerations;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingListCaptureQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListCaptureQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static ISubscriptionListener SubscriptionListener = new TestSubscriptionListener();


    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.AddRange(new object[] {
            new Request
            {
                Id = 1,
                UserId = UserContext.UserId,
                CaptureId = "001",
                SchemaVersion = "2.0",
                CaptureTime = DateTime.UtcNow,
                DocumentTime = DateTime.UtcNow,
                Events = new List<Event>{ new Event { Type = EventType.ObjectEvent } }
            },
            new Request
            {
                Id = 2,
                UserId = UserContext.UserId,
                CaptureId = "002",
                SchemaVersion = "2.0",
                CaptureTime = DateTime.UtcNow,
                DocumentTime = DateTime.UtcNow,
                Events = new List<Event>{ new Event { Type = EventType.ObjectEvent } }
            }
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheRequests()
    {
        var handler = new CaptureHandler(Context, UserContext, SubscriptionListener);
        var result = handler.ListCapturesAsync(Pagination.Max, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void ItShouldApplyThePaginationPerPageFilter()
    {
        var handler = new CaptureHandler(Context, UserContext, SubscriptionListener);
        var result = handler.ListCapturesAsync(new Pagination(1, 0), default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("001", result.First().CaptureId);
    }

    [TestMethod]
    public void ItShouldApplyThePaginationStartFromFilter()
    {
        var handler = new CaptureHandler(Context, UserContext, SubscriptionListener);
        var result = handler.ListCapturesAsync(new Pagination(int.MaxValue, 1), default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("002", result.First().CaptureId);
    }
}
