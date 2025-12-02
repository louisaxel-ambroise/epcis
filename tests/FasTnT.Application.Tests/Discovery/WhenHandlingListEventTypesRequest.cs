using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListEventTypesRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListEventTypesRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.Add(new Domain.Model.Request
        {
            RecordTime = DateTime.Now,
            DocumentTime = DateTime.Now,
            SchemaVersion = "2.0",
            UserId = "TESTUSER",
            Events =
            [
                new Event
                {
                    Type = Domain.Enumerations.EventType.ObjectEvent,
                    ReadPoint = "RP1"
                },
                new Event
                {
                    Type = Domain.Enumerations.EventType.TransformationEvent,
                    ReadPoint = "RP2"
                }
            ]
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public async Task ItShouldReturnAllTheReadPointsIfPageSizeIsGreaterThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(10, 0);

        var result = await handler.ListEventTypes(request, default);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public async Task ItShouldReturnTheRequestedNumberOfReadPointsIfPageSizeIsLowerThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(1, 0);

        var result = await handler.ListEventTypes(request, default);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public async Task ItShouldReturnTheCorrectPageOfData()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(10, 1);

        var result = await handler.ListEventTypes(request, default);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }
}
