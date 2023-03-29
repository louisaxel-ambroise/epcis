using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.DataSources;

[TestClass]
public class MasterDataQueryContextTests
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(MasterDataQueryContextTests));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassCleanup]
    public static void Cleanup()
    {
        if(Context != null)
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
                Masterdata = new List<MasterData> {
                    new MasterData
                    {
                        Id = "MD01",
                        Type = "urn:epcglobal:epcis:vtype:ReadPoint",
                        Attributes = new List<MasterDataAttribute>
                        {
                            new MasterDataAttribute
                            {
                                Id = "MD1AT1",
                                Value = "5"
                            },
                            new MasterDataAttribute
                            {
                                Id = "COMMON",
                                Value = "5"
                            }
                        }
                    },
                    new MasterData
                    {
                        Id = "MD03",
                        Type = "urn:epcglobal:epcis:vtype:BusinessLocation",
                        Attributes = new List<MasterDataAttribute>
                        {
                            new MasterDataAttribute
                            {
                                Id = "MD3AT1",
                                Value = "INNER"
                            }
                        }
                    },
                    new MasterData
                    {
                        Id = "MD02",
                        Type = "urn:epcglobal:epcis:vtype:BusinessLocation",
                        Children = new List<MasterDataChildren>
                        {
                            new MasterDataChildren
                            {
                                ChildrenId = "MD03",
                            }
                        },
                        Attributes = new List<MasterDataAttribute>
                        {
                            new MasterDataAttribute
                            {
                                Id = "MD2AT1",
                                Value = "VALUE"
                            },
                            new MasterDataAttribute
                            {
                                Id = "COMMON",
                                Value = "10"
                            }
                        }
                    }
                }
            }
        });

        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnTheStoredMasterdata()
    {
        var result = Context.QueryMasterData(Array.Empty<QueryParameter>()).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void ItShouldRestrictTheDataIfTheMaxElementCountLimitIsExceeded()
    {
        var result = Context.QueryMasterData(new[] { new QueryParameter { Name = "maxElementCount", Values = new[] { "1" } } }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void ItShouldApplyTheVocabularyNameParameter()
    {
        var result = Context.QueryMasterData(new[] { new QueryParameter { Name = "vocabularyName", Values = new[] { "urn:epcglobal:epcis:vtype:ReadPoint" } } }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("urn:epcglobal:epcis:vtype:ReadPoint", result.Single().Type);
    }

    [TestMethod]
    public void ItShouldApplyTheEQNameParameter()
    {
        var result = Context.QueryMasterData(new[] { new QueryParameter { Name = "EQ_name", Values = new[] { "MD02" } } }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("MD02", result.Single().Id);
    }

    [TestMethod]
    public void ItShouldApplyTheWDNameParameter()
    {
        var result = Context.QueryMasterData(new[] { new QueryParameter { Name = "WD_name", Values = new[] { "MD03" } } }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(x => x.Id == "MD02"));
        Assert.IsTrue(result.Any(x => x.Id == "MD03"));
    }

    [TestMethod]
    public void ItShouldApplyTheHasAttrParameter()
    {
        var result = Context.QueryMasterData(new[] { new QueryParameter { Name = "HASATTR", Values = new[] { "MD2AT1" } } }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    [DataRow("MD2AT1", "VALUE", 1)]
    [DataRow("MD2AT1", "NOVALUE", 0)]
    public void ItShouldApplyTheEqAttrParameter(string paramName, string value, int expectedResult)
    {
        var result = Context.QueryMasterData(new[] { new QueryParameter { Name = $"EQATTR_{paramName}", Values = new[] { value } } }).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResult, result.Count);
    }

    [TestMethod]
    [DataRow("UNKNOWN", true)]
    [DataRow("includeAttributes", false)]
    [DataRow("includeChildren", false)]
    [DataRow("EQ_userID", false)]
    [DataRow("WD_name", false)]
    [DataRow("attributeNames", false)]
    [DataRow("EQATTRS", true)]
    [DataRow("TEST", true)]
    public void ItShouldThrowAnExceptionIfTheParameterIsUnknown(string paramName, bool throws)
    {
        if (throws)
        {
            Assert.ThrowsException<EpcisException>(() => Context.QueryMasterData(new[] { new QueryParameter { Name = paramName, Values = new[] { "value" } } }).ToList());
        }
        else
        {
            var result = Context.QueryMasterData(new[] { new QueryParameter { Name = paramName, Values = new[] { "value" } } }).ToList();

            Assert.IsNotNull(result);
        }
    }
}
