using FasTnT.Domain.Model.Events;
using FasTnT.Host.Communication.Xml.Formatters;
using FasTnT.Host.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAnEmptySubscriptionResult
{
    public QueryResult Result = new(new("ExampleQueryName", new List<Event>(), "TestSubscription"));
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = SoapResponseFormatter.FormatQueryResult(Result);
    }

    [TestMethod]
    public void ItShouldReturnAnXElement()
    {
        Assert.IsNotNull(Formatted);
    }

    [TestMethod]
    public void TheXmlShouldBeCorrectlyFormatted()
    {
        Assert.IsTrue(Formatted.Name == XName.Get("QueryResults", "urn:epcglobal:epcis-query:xsd:1"));
        Assert.AreEqual(3, Formatted.Elements().Count());
        Assert.AreEqual(Result.Response.QueryName, Formatted.Element("queryName").Value);
        Assert.AreEqual(Result.Response.SubscriptionName, Formatted.Element("subscriptionID").Value);
        Assert.IsNotNull(Formatted.Element("resultsBody"));
    }
}
