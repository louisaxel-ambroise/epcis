using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.UseCases.StoreEpcisDocument;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingCaptureRequestThatDoesNotContainEvents
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    [ExpectedException(typeof(EpcisException))]
    public void ItShoultThrowAnException()
    {
        var handler = new StoreEpcisDocumentHandler(Context, UserContext);
        var request = new Request { SchemaVersion = "1.0" };

        try
        {
            var result = handler.StoreAsync(request, default).Result;
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException;
        }

    }
}

[TestClass]
public class WhenHandlingCaptureRequest
{
    readonly static EpcisContext Context = Tests.Context.EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnACaptureResultAndStoreTheRequest()
    {
        var handler = new StoreEpcisDocumentHandler(Context, UserContext);
        var request = new Request { SchemaVersion = "1.0", Events = new() { new Event { Type = EventType.ObjectEvent } } };
        var result = handler.StoreAsync(request, default).Result;
            
        Assert.IsNotNull(result);
        Assert.AreEqual(1, Context.Requests.Count());
    }
}
