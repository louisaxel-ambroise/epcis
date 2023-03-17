using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.DataSources;

[TestClass]
public class EventQueryContextTests
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(EventQueryContextTests));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static TestSubscriptionListener SubscriptionListener = new();

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.AddRange(new[]
        {
            new Request
            {
                CaptureId = "1",
                Id = 1,
                SchemaVersion = "2.0",
                Events = new List<Event> {
                    new Event
                    {
                        Type = EventType.ObjectEvent,
                        Action = EventAction.Add,
                        BusinessLocation = "test_location",
                        EventTime = DateTime.UtcNow,
                        EventTimeZoneOffset = "+02:00",
                        Epcs = new List<Epc>{ new Epc { Id = "epc1", Type = EpcType.List } }
                    },
                    new Event
                    {
                        Type = EventType.ObjectEvent,
                        Action = EventAction.Observe,
                        BusinessLocation = "test_location2",
                        EventTime = DateTime.UtcNow,
                        EventTimeZoneOffset = "+01:00",
                        Epcs = new List<Epc>{ new Epc { Id = "epc2", Type = EpcType.List } }
                    }
                }
            }
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheStoredEvents()
    {
        var result = Context.QueryEvents(Array.Empty<QueryParameter>()).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ItShouldRestrictTheDataIfTheEventCountLimitIsExceeded()
    {
        var result = Context.QueryEvents(new[] { new QueryParameter { Name = "eventCountLimit", Values = new[] { "1" } } }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void ItShouldRestrictTheDataIfTheMaxEventCountIsExceeded()
    {
        var result = Context.QueryEvents(new[] { new QueryParameter { Name = "maxEventCount", Values = new[] { "1" } } }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    [DataRow("UNKNOWN", true)]
    [DataRow("HASATTR_bizLocation_attrv1", false)]
    [DataRow("EQ_source_test", false)]
    [DataRow("EQ_userID", false)]
    [DataRow("EQ_bizLocation", false)]
    [DataRow("EQ_dataProcessingMethod", false)]
    [DataRow("EQATTRS", true)]
    [DataRow("TEST", true)]
    public void ItShouldThrowAnExceptionIfTheParameterIsUnknown(string paramName, bool throws)
    {
        if (throws)
        {
            Assert.ThrowsException<EpcisException>(() => Context.QueryEvents(new[] { new QueryParameter { Name = paramName, Values = new[] { "value" } } }).ToList());
        }
        else
        {
            var result = Context.QueryEvents(new[] { new QueryParameter { Name = paramName, Values = new[] { "value" } } }).ToList();

            Assert.IsNotNull(result);
        }
    }
}
