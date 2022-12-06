using FasTnT.Application.Relational;
using FasTnT.Application.Relational.UseCases.Captures;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using Moq;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static Mock<ISubscriptionListener> SubscriptionListener = new(MockBehavior.Loose);

    [TestMethod]
    public void ItShouldReturnACaptureResultAndStoreTheRequest()
    {
        var handler = new CaptureUseCasesHandler(Context, UserContext, SubscriptionListener.Object);
        var request = new Request { SchemaVersion = "1.0", Events = new() { new Event { Type = EventType.ObjectEvent } } };
        var result = handler.StoreAsync(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, Context.Set<Request>().Count());
    }
}
