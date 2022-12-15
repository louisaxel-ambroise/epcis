using FasTnT.Application.Database;
using FasTnT.Application.Services.DataSources;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries.Parameters;

[TestClass]
public class WhenSimpleEventQueryReturnsMoreThanMaxEventCountParameter
{
    public EpcisContext Context { get; set; }
    public IEpcisDataSource Query { get; set; }
    public QueryParameter Parameter { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        Context = Tests.Context.EpcisTestContext.GetContext("simpleEventQuery");
        Query = new EventDataSource(Context);

        Context.Add(new Domain.Model.Request
        {
            Events = new[] {
                new Event
                {
                    Action = Domain.Enumerations.EventAction.Observe
                },
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

        Parameter = QueryParameter.Create("maxEventCount", new[] { "1" });
    }

    [TestMethod]
    public void ItShouldThrowAQueryTooLargeExceptionException()
    {
        var catched = default(Exception);

        try
        {
            Query.Apply(Parameter);
            var result = Query.ExecuteAsync(default).Result;
            Assert.IsFalse(true, "The query should fail");
        }
        catch (Exception ex)
        {
            catched = ex is AggregateException ? ex.InnerException : ex;
        }

        Assert.IsNotNull(catched);
        Assert.IsInstanceOfType(catched, typeof(EpcisException));
        Assert.AreEqual(ExceptionType.QueryTooLargeException, ((EpcisException)catched).ExceptionType);
    }
}
