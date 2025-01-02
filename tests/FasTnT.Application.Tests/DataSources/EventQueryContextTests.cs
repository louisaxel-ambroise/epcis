using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.DataSources;

[TestClass]
public class EventQueryContextTests
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(EventQueryContextTests));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        Context?.Database.EnsureDeleted();
    }

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.AddRange(
        [
            new Request
            {
                CaptureId = "capture_id_test",
                RecordTime =  new DateTime(2020, 03, 15, 21, 14, 10, DateTimeKind.Utc),
                Id = 1,
                SchemaVersion = "2.0",
                Masterdata = [
                    new MasterData
                    {
                        Type = MasterData.Location,
                        Id = "test_location2"
                    },
                    new MasterData
                    {
                        Type = MasterData.Location,
                        Id = "test_location",
                        Children = [new MasterDataChildren
                        {
                            ChildrenId = "test_location2"
                        }]
                    },
                    new MasterData
                    {
                        Type = MasterData.ReadPoint,
                        Id = "rp_test2"
                    },
                    new MasterData
                    {
                        Type = MasterData.ReadPoint,
                        Id = "rp_test",
                        Children = [new MasterDataChildren
                        {
                            ChildrenId = "rp_test2"
                        }]
                    }
                ],
                Events = [
                    new Event
                    {
                        Type = EventType.ObjectEvent,
                        Action = EventAction.Add,
                        BusinessLocation = "test_location",
                        ReadPoint = "rp_test2",
                        CorrectiveDeclarationTime = new DateTime(2024, 02, 15, 21, 14, 10),
                        CorrectiveReason = "invalid_evt",
                        CorrectiveEventIds = new List<CorrectiveEventId>{{ new CorrectiveEventId { CorrectiveId = "ni://test2" } }},
                        BusinessStep = "step",
                        EventId = "ni://test",
                        EventTime =  new DateTime(2020, 02, 15, 21, 14, 10),
                        EventTimeZoneOffset = "+02:00",
                        Epcs = [new Epc { Id = "epc1", Type = EpcType.List }]
                    },
                    new Event
                    {
                        Type = EventType.AssociationEvent,
                        Action = EventAction.Observe,
                        BusinessLocation = "test_location2",
                        TransformationId = "transformationIdEvent",
                        ReadPoint = "rp_test",
                        Disposition = "disposition",
                        EventTime = new DateTime(2021, 02, 15, 21, 14, 10),
                        EventTimeZoneOffset = "+01:00",
                        Epcs = [ new Epc { Id = "epc2", Type = EpcType.List }, new Epc { Id = "epc.value.1", Type = EpcType.List }],
                        Fields =
                        [
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
                        ]
                    }
                ]
            }
        ]);

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
        var result = Context.QueryEvents([new QueryParameter { Name = "eventCountLimit", Values = ["1"] }]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void ItShouldRestrictTheDataIfTheMaxEventCountIsExceeded()
    {
        var result = Context.QueryEvents([new QueryParameter { Name = "maxEventCount", Values = ["1"] }]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyTheeventTypeFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("eventType", "ObjectEvent")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Type == EventType.ObjectEvent));
    }

    [TestMethod]
    public void ItShouldApplyTheGE_eventTimeFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("GE_eventTime", "2021-01-12T10:24:10Z")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.EventTime >= new DateTime(2021, 01, 12, 10, 24, 10)));
    }

    [TestMethod]
    public void ItShouldApplyTheLT_eventTimeFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("LT_eventTime", "2021-01-12T10:24:10Z")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.EventTime < new DateTime(2021, 01, 12, 10, 24, 10)));
    }

    [TestMethod]
    public void ItShouldApplyTheGE_recordTimeFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("GE_recordTime", "2021-03-12T10:24:10Z")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
        Assert.IsTrue(result.All(x => x.Request.RecordTime >= new DateTime(2021, 03, 12, 10, 24, 10)));
    }

    [TestMethod]
    public void ItShouldApplyTheLT_recordTimeFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("LT_recordTime", "2021-03-12T10:24:10Z")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(x => x.Request.RecordTime < new DateTime(2021, 03, 12, 10, 24, 10)));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_actionFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_action", "ADD")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Action == EventAction.Add));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_bizLocationFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_bizLocation", "test_location", "test_location2")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(x => x.BusinessLocation == "test_location" || x.BusinessLocation == "test_location2"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_bizStepFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_bizStep", "step")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.BusinessStep == "step"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_dispositionFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_disposition", "disposition")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Disposition == "disposition"));
    }

    [TestMethod]
    public void ItShouldApplyTheorderByFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("orderBy", "eventTime")]).ToList();
        var sorted = result.OrderByDescending(s => s.EventTime);

        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
    }

    [TestMethod]
    public void ItShouldApplyTheorderDirectionFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("orderBy", "eventTime"), QueryParameter.Create("orderDirection", "ASC")]).ToList();
        var sorted = result.OrderBy(s => s.EventTime);

        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(sorted.ToList(), result.ToList());
    }

    [TestMethod]
    public void ItShouldApplyTheNextPageTokenFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("nextPageToken", "1")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyThePerPageFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("perPage", "1")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_eventIDFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_eventID", "ni://test")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.EventId == "ni://test"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_readPointFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_readPoint", "rp_test")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.ReadPoint == "rp_test"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_transformationIDFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_transformationID", "transformationIdEvent")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.ReadPoint == "rp_test"));
    }

    [TestMethod]
    public void ItShouldApplyTheEXISTS_errorDeclarationFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EXISTS_errorDeclaration", "")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.CorrectiveDeclarationTime.HasValue));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_errorReasonFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_errorReason", "invalid_evt")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.CorrectiveReason == "invalid_evt"));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_correctiveEventIDFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_correctiveEventID", "ni://test2")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.CorrectiveEventIds.Any(c => c.CorrectiveId == "ni://test2")));
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_requestIDFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_requestID", "1")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyTheEQ_captureIDFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("EQ_captureID", "capture_id_test")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyTheInnerIlmdFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("GE_INNER_ILMD_test#numeric", "5")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Fields.Any(f => f.Namespace == "test" && f.Name == "numeric" && f.ParentIndex.HasValue && f.NumericValue >= 5)));
    }

    [TestMethod]
    public void ItShouldApplyTheIlmdFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("LT_ILMD_test#numeric", "3")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Fields.Any(f => f.Namespace == "test" && f.Name == "numeric" && !f.ParentIndex.HasValue && f.NumericValue < 3)));
    }

    [TestMethod]
    public void ItShouldApplyTheMATCH_anyEpcFilter()
    {
        var result = Context.QueryEvents([QueryParameter.Create("MATCH_anyEPC", "epc.*")]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(x => x.Epcs.Any(e => e.Id.StartsWith("epc."))));
    }

    [TestMethod]
    [DataRow("test_location2", 2)]
    [DataRow("test_location", 1)]
    [DataRow("test_unknownlocation", 0)]
    public void ItShouldApplyTheWD_bizLocationFilter(string value, int expected)
    {
        var result = Context.QueryEvents([QueryParameter.Create("WD_bizLocation", value)]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(expected, result.Count);
    }

    [TestMethod]
    [DataRow("rp_test2", 2)]
    [DataRow("rp_test", 1)]
    [DataRow("rp_unknown", 0)]
    public void ItShouldApplyTheWD_readPointFilter(string value, int expected)
    {
        var result = Context.QueryEvents([QueryParameter.Create("WD_readPoint", value)]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(expected, result.Count);
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
            Assert.ThrowsException<EpcisException>(() => Context.QueryEvents([new QueryParameter { Name = paramName, Values = ["value"] }]).ToList());
        }
        else
        {
            var result = Context.QueryEvents([new QueryParameter { Name = paramName, Values = ["value"] }]).ToList();

            Assert.IsNotNull(result);
        }
    }

    [TestMethod]
    [DataRow("2023-01-15T00:00:00.000Z", 1)]
    [DataRow("2024-02-15T21:14:10.000Z", 1)]
    [DataRow("2025-01-10T00:00:00.000Z", 0)]
    public void ItShouldApplyTheGE_errorDeclarationTimeFilter(string value, int expectedEvents)
    {
        var result = Context.QueryEvents([QueryParameter.Create("GE_errorDeclarationTime", value)]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedEvents, result.Count);
        Assert.IsTrue(result.All(x => x.CorrectiveDeclarationTime >= DateTime.Parse(value).ToUniversalTime()));
    }

    [TestMethod]
    [DataRow("2025-01-10T00:00:00.000Z", 1)]
    [DataRow("2024-02-15T21:14:10.000Z", 0)]
    [DataRow("2023-01-15T00:00:00.000Z", 0)]
    public void ItShouldApplyTheLT_errorDeclarationTimeFilter(string value, int expectedEvents)
    {
        var result = Context.QueryEvents([QueryParameter.Create("LT_errorDeclarationTime", value)]).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedEvents, result.Count);
        Assert.IsTrue(result.All(x => x.CorrectiveDeclarationTime < DateTime.Parse(value).ToUniversalTime()));
    }
}
