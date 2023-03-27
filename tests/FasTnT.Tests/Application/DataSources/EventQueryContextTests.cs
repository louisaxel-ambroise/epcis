using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Model.Events;

namespace FasTnT.Application.Tests.DataSources;

[TestClass]
public class EventQueryContextTests
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(EventQueryContextTests));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

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
                CaptureId = "capture_id_test",
                Id = 1,
                SchemaVersion = "2.0",
                Events = new List<Event> {
                    new Event
                    {
                        Type = EventType.ObjectEvent,
                        Action = EventAction.Add,
                        BusinessLocation = "test_location",
                        CorrectiveDeclarationTime = DateTime.UtcNow,
                        CorrectiveReason = "invalid_evt",
                        CorrectiveEventIds = new List<CorrectiveEventId>{{ new CorrectiveEventId { CorrectiveId = "ni://test2" } }},
                        BusinessStep = "step",
                        EventId = "ni://test",
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
                        TransformationId = "transformationIdEvent",
                        ReadPoint = "rp_test",
                        Disposition = "disposition",
                        CaptureTime = new DateTime(2021, 03, 15, 21, 14, 10),
                        EventTime = new DateTime(2021, 02, 15, 21, 14, 10),
                        EventTimeZoneOffset = "+01:00",
                        Epcs = new List<Epc>{ new Epc { Id = "epc2", Type = EpcType.List }, new Epc { Id = "epc.value.1", Type = EpcType.List } },
                        Fields = new List<Field>
                        {
                            new Field
                            {
                                Index = 1,
                                Type = FieldType.Ilmd,
                                Name = "container",
                                Namespace = "test"
                            },
                            new Field
                            {
                                Index = 2,
                                ParentIndex = 1,
                                Type = FieldType.Ilmd,
                                Name = "numeric",
                                Namespace = "test",
                                NumericValue = 6.2,
                                TextValue = "6.2"
                            },
                            new Field
                            {
                                Index = 3,
                                Type = FieldType.Ilmd,
                                Name = "numeric",
                                Namespace = "test",
                                NumericValue = 2.5,
                                TextValue = "2.5"
                            }
                        }
                    }
                }
            }
        }); ;

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
    public void ItShouldApplyTheEQ_eventIDFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_eventID", "ni://test") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.EventId == "ni://test"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_readPointFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_readPoint", "rp_test") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.ReadPoint == "rp_test"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_transformationIDFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_transformationID", "transformationIdEvent") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.ReadPoint == "rp_test"));
    }

    [TestMethod]
    public void ItShouldApplyTheEXISTS_errorDeclarationFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EXISTS_errorDeclaration", "") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.CorrectiveDeclarationTime.HasValue));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_errorReasonFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_errorReason", "invalid_evt") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.CorrectiveReason == "invalid_evt"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_correctiveEventIDFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_correctiveEventID", "ni://test2") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.CorrectiveEventIds.Any(c => c.CorrectiveId == "ni://test2")));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_requestIDFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_requestID", "1") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_captureIDFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("EQ_captureID", "capture_id_test") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyTheInnerIlmdFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("GE_INNER_ILMD_test#numeric", "5") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Fields.Any(f => f.Namespace == "test" && f.Name == "numeric" && f.ParentIndex.HasValue && f.NumericValue >= 5)));
    }

    [TestMethod]
    public void ItShouldApplyTheIlmdFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("LT_ILMD_test#numeric", "3") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Fields.Any(f => f.Namespace == "test" && f.Name == "numeric" && !f.ParentIndex.HasValue && f.NumericValue < 3)));
    }

    [TestMethod]
    public void ItShouldApplyTheMATCH_anyEpcFilter()
    {
        var result = Context.QueryEvents(new[] { QueryParameter.Create("MATCH_anyEPC", "epc.*") }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Epcs.Any(e => e.Id.StartsWith("epc."))));
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
