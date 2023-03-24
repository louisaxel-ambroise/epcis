using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Host.Features.v2_0.Communication.Xml.Formatters;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingAnEmptyQueryResponse
{
    public QueryResponse Result = new("ExampleQueryName", QueryData.Empty);
    public string Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = XmlResponseFormatter.Format(new QueryResult(Result));
    }

    [TestMethod]
    public void ItShouldReturnAnXElement()
    {
        Assert.IsNotNull(XElement.Parse(Formatted));
    }

    [TestMethod]
    public void TheXmlShouldBeCorrectlyFormatted()
    {
        var element = XElement.Parse(Formatted);

        Assert.IsTrue(element.Name == XName.Get("QueryResults", "urn:epcglobal:epcis-query:xsd:1"));
        Assert.AreEqual(2, element.Elements().Count());
        Assert.AreEqual(Result.QueryName, element.Element("queryName").Value);
        Assert.IsNotNull(element.Element("resultsBody"));
    }

    [TestMethod]
    public void ThereShouldNotBeASubscriptionIDField()
    {
        var element = XElement.Parse(Formatted);

        Assert.IsNull(element.Element("subscriptionID"));
    }
}
