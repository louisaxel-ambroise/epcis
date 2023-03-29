using FasTnT.Host.Features.v2_0.Communication.Formatters;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingAnInvalidQueryResult
{
    public QueryResponse Result = new("ExampleQueryName", new QueryData { EventList = null, VocabularyList = null });
    public string Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
    }

    [TestMethod]
    public void ItShouldThrowANotImplementedException()
    {
        Assert.ThrowsException<NotImplementedException>(() => XmlResponseFormatter.Format(new QueryResult(Result)));
    }
}
