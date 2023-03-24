using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Storage;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListReadPointsRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListReadPointsRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

    [TestInitialize]
    public void Initialize()
    {
        Context.Add(new Domain.Model.Request
        {
            CaptureTime = DateTime.Now,
            DocumentTime = DateTime.Now,
            SchemaVersion = "2.0",
            UserId = "TESTUSER",
            Events = new List<Event>
            {
                new Event
                {
                    ReadPoint = "RP1"
                },
                new Event
                {
                    ReadPoint = "RP2"
                }
            }
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAllTheReadPointsIfPageSizeIsGreaterThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(10, 0);

        var result = handler.ListReadPoints(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheRequestedNumberOfReadPointsIfPageSizeIsLowerThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(1, 0);

        var result = handler.ListReadPoints(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheCorrectPageOfData()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(10, 1);

        var result = handler.ListReadPoints(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }
}
