using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Application.UseCases.Queries;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Tests.Queries;

[TestClass]
public class WhenHandlingGetQueryNamesQuery
{
    readonly static EpcisContext Context = EpcisTestContext.GetContext(nameof(WhenHandlingGetQueryNamesQuery));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        Context.AddRange(new[] {
            new StoredQuery
            {
                Id = 1,
                Name = "QueryOne"
            },
            new StoredQuery
            {
                Id = 2,
                Name = "QueryTwo"
            }
        });
        Context.SaveChanges();
    }

    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var handler = new QueriesUseCasesHandler(Context, UserContext);
        var result = handler.ListQueriesAsync(Pagination.Max, CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(2, result.Count());
        CollectionAssert.AreEquivalent(new[] { "QueryOne", "QueryTwo" }, result.Select(x => x.Name).ToArray());
    }

    [TestMethod]
    public void ItShouldApplyThePaginationPerPageFilter()
    {
        var handler = new QueriesUseCasesHandler(Context, UserContext);
        var result = handler.ListQueriesAsync(new Pagination(1, 0), CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(1, result.Count());
        CollectionAssert.AreEquivalent(new[] { "QueryOne" }, result.Select(x => x.Name).ToArray());
    }

    [TestMethod]
    public void ItShouldApplyThePaginationStartFromFilter()
    {
        var handler = new QueriesUseCasesHandler(Context, UserContext);
        var result = handler.ListQueriesAsync(new Pagination(30, 1), CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(List<StoredQuery>));

        Assert.AreEqual(1, result.Count());
        CollectionAssert.AreEquivalent(new[] { "QueryTwo" }, result.Select(x => x.Name).ToArray());
    }
}
