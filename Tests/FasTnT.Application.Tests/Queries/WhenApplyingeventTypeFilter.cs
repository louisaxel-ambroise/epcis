using FasTnT.Application.Queries;
using FasTnT.Domain.Queries;
using FasTnT.Infrastructure.Store;

namespace FasTnT.Application.Tests.Queries
{
    [TestClass]
    public class WhenApplyingeventTypeFilter
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
                        Type = Domain.Enumerations.EventType.ObjectEvent,
                        Action = Domain.Enumerations.EventAction.Observe
                    },
                    new Domain.Model.Event
                    {
                        Type = Domain.Enumerations.EventType.TransactionEvent,
                        Action = Domain.Enumerations.EventAction.Observe
                    }
                }.ToList(),
                CaptureDate = DateTime.Now,
                DocumentTime = DateTime.Now,
                SchemaVersion = "1.2"
            });
            Context.SaveChanges();

            Parameters = new[] { new QueryParameter("eventType", new[] { "ObjectEvent" }) }.ToList();
        }

        [TestMethod]
        public void ItShouldOnlyReturnTheEventsOfTheSpecifiedType()
        {
            var result = Query.HandleAsync(Parameters, default).Result;
            Assert.AreEqual(1, result.EventList.Count);
            Assert.IsTrue(result.EventList.All(x => x.Type == Domain.Enumerations.EventType.ObjectEvent));
        }
    }
}
