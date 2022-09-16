using FasTnT.Application.Services.Queries;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries
{
    [TestClass]
    public class WhenSimpleEventQueryReturnsMoreThanMaxEventCountParameter
    {
        public EpcisContext Context { get; set; }
        public IEpcisDataSource Query { get; set; }
        public IList<QueryParameter> Parameters { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Context = Tests.Context.EpcisTestContext.GetContext("simpleEventQuery");
            Query = new SimpleEventQuery();

            Context.Requests.Add(new Domain.Model.Request
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
                CaptureDate = DateTime.Now,
                DocumentTime = DateTime.Now,
                SchemaVersion = "1.2"
            });
            Context.SaveChanges();

            Parameters = new[] { QueryParameter.Create("maxEventCount", new[] { "1" }) }.ToList();
        }

        [TestMethod]
        public void ItShouldThrowAQueryTooLargeExceptionException()
        {
            var catched = default(Exception);

            try
            {
                Query.ExecuteAsync(Context, Parameters, default).Wait();
                Assert.IsFalse(true, "The query should fail");
            }
            catch(Exception ex)
            {
                catched = ex is AggregateException ? ex.InnerException : ex;
            }

            Assert.IsNotNull(catched);
            Assert.IsInstanceOfType(catched, typeof(EpcisException));
            Assert.AreEqual(((EpcisException)catched).ExceptionType, ExceptionType.QueryTooLargeException);
        }
    }
}
