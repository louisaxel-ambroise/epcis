using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Communication.Xml.Formatters;
using FasTnT.Host.Endpoints.Interfaces;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.XML;

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
        Assert.Throws<NotImplementedException>(() => XmlResponseFormatter.Format(new QueryResult(Result)));
    }
}
