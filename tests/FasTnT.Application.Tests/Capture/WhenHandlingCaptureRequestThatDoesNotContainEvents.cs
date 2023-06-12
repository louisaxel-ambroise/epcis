using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.Handlers;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using Microsoft.Extensions.Options;
using FasTnT.Domain;

namespace FasTnT.Application.Tests.Capture;

[TestClass]
public class WhenHandlingCaptureRequestThatDoesNotContainEvents
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [TestMethod]
    [ExpectedException(typeof(EpcisException))]
    public void ItShoultThrowAnException()
    {
        var handler = new CaptureHandler(Context, UserContext, Options.Create(new Constants()));
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
