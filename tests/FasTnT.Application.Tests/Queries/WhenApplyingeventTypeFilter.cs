using FasTnT.Application.EfCore.Services.Queries;
using FasTnT.Application.EfCore.Store;
using FasTnT.Application.Services.Queries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries
{
    [TestClass]
    public class WhenApplyingeventTypeFilter
    {
        public EpcisContext Context { get; set; }
        public IEpcisDataSource Query { get; set; }
        public IList<QueryParameter> Parameters { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Context = Tests.Context.EpcisTestContext.GetContext("simpleEventQuery");
            Query = new SimpleEventQuery(Context);

            Context.Requests.Add(new Domain.Model.Request
            {
                Events = new[] {
                    new Event
                    {
                        Type = Domain.Enumerations.EventType.ObjectEvent,
                        Action = Domain.Enumerations.EventAction.Observe
                    },
                    new Event
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

            Parameters = new[] { QueryParameter.Create("eventType", "ObjectEvent") }.ToList();
        }

        [TestMethod]
        public void ItShouldOnlyReturnTheEventsOfTheSpecifiedType()
        {
            var result = Query.ExecuteAsync(Parameters, CancellationToken.None).Result;
            Assert.AreEqual(1, result.EventList.Count);
            Assert.IsTrue(result.EventList.All(x => x.Type == Domain.Enumerations.EventType.ObjectEvent));
        }
    }
}
