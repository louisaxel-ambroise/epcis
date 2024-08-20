using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListEpcsRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListEpcsRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }

    [TestInitialize]
    public void Initialize()
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
                    Epcs = [
                        new Epc { Type = Domain.Enumerations.EpcType.List, Id = "test:epc:1" },
                        new Epc { Type = Domain.Enumerations.EpcType.List, Id = "test:epc:2" },
                        new Epc { Type = Domain.Enumerations.EpcType.List, Id = "test:epc:3" }
                    ]
                }
            ]
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAllTheEpcsIfPageSizeIsGreaterThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(10, 0);

        var result = handler.ListEpcs(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheRequestedNumberOfEpcsIfPageSizeIsLowerThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(1, 0);

        var result = handler.ListEpcs(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheCorrectPageOfData()
    {
        var handler = new TopLevelResourceHandler(Context, UserContext);
        var request = new Pagination(3, 2);

        var result = handler.ListEpcs(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }
}
