using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListBizLocationsRequest
{
    readonly static EpcisTestContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListBizLocationsRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.DbContext.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.DbContext.Add(new Domain.Model.Request
        {
            RecordTime = DateTime.Now,
            DocumentTime = DateTime.Now,
            SchemaVersion = "2.0",
            UserId = "TESTUSER",
            Events =
            [
                new Event
                {
                    BusinessLocation = "BL1"
                },
                new Event
                {
                    BusinessLocation = "BL2"
                }
            ]
        });

        Context.DbContext.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAllTheBizLocationsIfPageSizeIsGreaterThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context.DbContext, UserContext);
        var request = new Pagination(10, 0);

        var result = handler.ListBizLocations(request, default).Result;

        Assert.IsNotNull(result);
        Assert.HasCount(2, result);
    }

    [TestMethod]
    public void ItShouldReturnTheRequestedNumberOfBizLocationsIfPageSizeIsLowerThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context.DbContext, UserContext);
        var request = new Pagination(1, 0);

        var result = handler.ListBizLocations(request, default).Result;

        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
    }

    [TestMethod]
    public void ItShouldReturnTheCorrectPageOfData()
    {
        var handler = new TopLevelResourceHandler(Context.DbContext, UserContext);
        var request = new Pagination(10, 1);

        var result = handler.ListBizLocations(request, default).Result;

        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
    }
}
