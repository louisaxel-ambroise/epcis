using FasTnT.Application.Relational;
using FasTnT.Application.Relational.UseCases.TopLevelResources;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Discovery;

[TestClass]
public class WhenHandlingListReadPointsRequest
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingListReadPointsRequest));

    [TestInitialize]
    public void Initialize()
    {
        Context.Add(new Domain.Model.Request
        {
            CaptureDate = DateTimeOffset.Now,
            DocumentTime = DateTimeOffset.Now,
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
        var handler = new TopLevelResourceUseCasesHandler(Context);
        var request = new Pagination(10, 0);

        var result = handler.ListReadPoints(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheRequestedNumberOfReadPointsIfPageSizeIsLowerThanNumberOfEpcs()
    {
        var handler = new TopLevelResourceUseCasesHandler(Context);
        var request = new Pagination(1, 0);

        var result = handler.ListReadPoints(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void ItShouldReturnTheCorrectPageOfData()
    {
        var handler = new TopLevelResourceUseCasesHandler(Context);
        var request = new Pagination(10, 1);

        var result = handler.ListReadPoints(request, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }
}
