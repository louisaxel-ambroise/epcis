using FasTnT.Host.Features.v2_0.Communication.Formatters;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingAnEmptySubscriptionResult
{
    public QueryResponse Result = new("ExampleQueryName", "ASubscription", QueryData.Empty);
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
        Assert.AreEqual(3, element.Elements().Count());
        Assert.AreEqual(Result.QueryName, element.Element("queryName").Value);
        Assert.AreEqual(Result.SubscriptionId, element.Element("subscriptionID").Value);
        Assert.IsNotNull(element.Element("resultsBody"));
    }
}
