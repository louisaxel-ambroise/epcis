using FasTnT.Application.Database;
using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Queries.DataSources;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries.Parameters;

[TestClass]
public class WhenSimpleEventQueryReturnsLessThanMaxEventCountParameter
{
    public EpcisContext Context { get; set; }
    public IEpcisDataSource Query { get; set; }
    public IList<QueryParameter> Parameters { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Context = Tests.Context.EpcisTestContext.GetContext("simpleEventQuery");
        Query = new SimpleEventQuery(Context);

        Context.Add(new Domain.Model.Request
        {
            Events = new[] {
                new Event
                {
                    Action = Domain.Enumerations.EventAction.Observe
                }
            }.ToList(),
            CaptureTime = DateTime.Now,
            DocumentTime = DateTime.Now,
            SchemaVersion = "1.2"
        });
        Context.SaveChanges();

        Parameters = new[] { QueryParameter.Create("maxEventCount", new[] { "2" }) }.ToList();
    }

    [TestMethod]
    public void ItShouldThrowAQueryTooLargeExceptionException()
    {
        var result = Query.ExecuteAsync(Parameters, default).Result;

        Assert.IsNotNull(result);
        Assert.AreEqual(result.EventList.Count, 1);
    }
}
