using FasTnT.Application.Queries;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;
using FasTnT.Infrastructure.Store;

namespace FasTnT.Application.Tests.Queries
{
    [TestClass]
    public class WhenSimpleEventQueryReturnsMoreThanMaxEventCountParameter
    {
        public EpcisContext Context { get; set; }
        public Services.IEpcisQuery Query { get; set; }
        public IList<QueryParameter> Parameters { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Context = Tests.Context.TestContext.GetContext("simpleEventQuery");
            Query = new SimpleEventQuery(Context);

            Context.Requests.Add(new Domain.Model.Request
            {
                Events = new[] {
                    new Domain.Model.Event
                    {
                        Action = Domain.Enumerations.EventAction.Observe
                    },
                    new Domain.Model.Event
                    {
                        Action = Domain.Enumerations.EventAction.Observe
                    }
                }.ToList(),
                CaptureDate = DateTime.Now,
                DocumentTime = DateTime.Now,
                SchemaVersion = "1.2"
            });
            Context.SaveChanges();

            Parameters = new[] { new QueryParameter("maxEventCount", new[] { "1" }) }.ToList();
        }

        [TestMethod]
        public void ItShouldThrowAQueryTooLargeExceptionException()
        {
            var catched = default(Exception);

            try
            {
                Query.HandleAsync(Parameters, default).Wait();
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
