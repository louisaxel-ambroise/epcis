using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v2_0.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v2_0.Communication;

[TestClass]
public class WhenFormattingAnEmptyQueryResponse
{
    public QueryResponse Result = new("ExampleQueryName", QueryData.Empty);
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = XmlResponseFormatter.FormatPoll(Result);
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
        Assert.AreEqual(2, Formatted.Elements().Count());
        Assert.AreEqual(Result.QueryName, Formatted.Element("queryName").Value);
        Assert.IsNotNull(Formatted.Element("resultsBody"));
    }

    [TestMethod]
    public void ThereShouldNotBeASubscriptionIDField()
    {
        Assert.IsNull(Formatted.Element("subscriptionID"));
    }
}
