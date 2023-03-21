using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using System.Globalization;

namespace FasTnT.Application.Tests.DataSources;

[TestClass]
public class EventQueryContextTests
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(EventQueryContextTests));
    readonly static ICurrentUser UserContext = new TestCurrentUser();
    readonly static TestSubscriptionListener SubscriptionListener = new();

    [ClassCleanup]
    public static void Cleanup()
    {
        if (Context != null)
        {
            Context.Database.EnsureDeleted();
        }
    }

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
                        BusinessStep = "step",
                        CaptureTime =  new DateTime(2020, 03, 15, 21, 14, 10),
                        EventTime =  new DateTime(2020, 02, 15, 21, 14, 10),
                        EventTimeZoneOffset = "+02:00",
                        Epcs = new List<Epc>{ new Epc { Id = "epc1", Type = EpcType.List } }
                    },
                    new Event
                    {
                        Type = EventType.AssociationEvent,
                        Action = EventAction.Observe,
                        BusinessLocation = "test_location2",
                        Disposition = "disposition",
                        CaptureTime = new DateTime(2021, 03, 15, 21, 14, 10),
                        EventTime = new DateTime(2021, 02, 15, 21, 14, 10),
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
    public void ItShouldApplyTheeventTypeFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("eventType", "ObjectEvent") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Type == EventType.ObjectEvent));
    }

    [TestMethod]
    public void ItShouldApplyTheGE_eventTimeFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("GE_eventTime", "2021-01-12T10:24:10Z") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.EventTime >= new DateTime(2021, 01, 12, 10, 24, 10)));
    }

    [TestMethod]
    public void ItShouldApplyTheLT_eventTimeFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("LT_eventTime", "2021-01-12T10:24:10Z") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.EventTime < new DateTime(2021, 01, 12, 10, 24, 10)));
    }

    [TestMethod]
    public void ItShouldApplyTheGE_recordTimeFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("GE_recordTime", "2021-03-12T10:24:10Z") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.CaptureTime >= new DateTime(2021, 03, 12, 10, 24, 10)));
    }

    [TestMethod]
    public void ItShouldApplyTheLT_recordTimeFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("LT_recordTime", "2021-03-12T10:24:10Z") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.CaptureTime < new DateTime(2021, 03, 12, 10, 24, 10)));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_actionFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_action", "ADD") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Action == EventAction.Add));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_bizLocationFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_bizLocation", "test_location", "test_location2") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(x => x.BusinessLocation == "test_location" || x.BusinessLocation == "test_location2"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_bizStepFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_bizStep", "step") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.BusinessStep == "step"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_dispositionFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_disposition", "disposition") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Disposition == "disposition"));
    }

    [TestMethod]
    public void ItShouldApplyTheorderByFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("orderBy", "eventTime") }).ToList();
        var sorted = result.OrderByDescending(s => s.EventTime);

        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
    }

    [TestMethod]
    public void ItShouldApplyTheorderDirectionFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("orderBy", "eventTime"), QueryParameter.Create("orderDirection", "ASC") }).ToList();
        var sorted = result.OrderBy(s => s.EventTime);

        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
    }

    [TestMethod]
    public void ItShouldApplyTheNextPageTokenFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("nextPageToken", "1") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyThePerPageFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("perPage", "1") }).ToList();

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
