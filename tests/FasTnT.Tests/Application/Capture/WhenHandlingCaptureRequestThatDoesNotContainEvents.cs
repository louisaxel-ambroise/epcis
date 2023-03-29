using FasTnT.Application.Handlers;
using FasTnT.Tests.Application.Context;

namespace FasTnT.Tests.Application.Capture;

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
        var handler = new CaptureHandler(Context, UserContext);
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
