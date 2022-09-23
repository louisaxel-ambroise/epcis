using FasTnT.Application.EfCore.Services.Queries;
using FasTnT.Application.EfCore.Store;
using FasTnT.Application.Services.Queries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries
{
    [TestClass]
    public class WhenApplyingGE_eventTimeFilter
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
                        EventTime = new (2021, 01, 12, 10, 24, 10, DateTimeKind.Utc),
                        Action = Domain.Enumerations.EventAction.Observe
                    },
                    new Event
                    {
                        Type = Domain.Enumerations.EventType.TransactionEvent,
                        EventTime = new (2021, 01, 12, 10, 30, 00, DateTimeKind.Utc),
                        Action = Domain.Enumerations.EventAction.Observe
                    },
                    new Event
                    {
                        Type = Domain.Enumerations.EventType.TransactionEvent,
                        EventTime = new (2011, 08, 02, 21, 50, 00, DateTimeKind.Utc),
                        Action = Domain.Enumerations.EventAction.Observe
                    }
                }.ToList(),
                CaptureDate = DateTime.Now,
                DocumentTime = DateTime.Now,
                SchemaVersion = "1.2"
            });
            Context.SaveChanges();

            Parameters = new[] { QueryParameter.Create("GE_eventTime", new[] { "2020-01-12T10:24:10.000Z" }) }.ToList();
        }

        [TestMethod]
        public void ItShouldOnlyReturnTheEventsCaptureAfterOrOnTheDate()
        {
            var result = Query.ExecuteAsync(Parameters, default).Result;
            Assert.AreEqual(2, result.EventList.Count);
            Assert.IsTrue(result.EventList.All(x => x.EventTime >= new DateTime(2021, 01, 12, 10, 24, 10, DateTimeKind.Utc)));
        }
    }
}
