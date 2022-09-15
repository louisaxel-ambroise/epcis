using FasTnT.Application.Services.Queries;
using FasTnT.Application.UseCases.GetStandardQueryNames;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetQueryNamesQuery
{
    [TestMethod]
    public void ItShouldReturnAllTheQueryNames()
    {
        var queries = new IStandardQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        var handler = new GetStandardQueryNamesHandler(queries);
        var result = handler.GetQueryNamesAsync(CancellationToken.None).Result;

        Assert.IsInstanceOfType(result, typeof(IEnumerable<string>));

        Assert.AreEqual(2, result.Count());
        CollectionAssert.AreEquivalent(new[] { "SimpleEventQuery", "SimpleMasterDataQuery" }, result.ToArray());
    }
}
