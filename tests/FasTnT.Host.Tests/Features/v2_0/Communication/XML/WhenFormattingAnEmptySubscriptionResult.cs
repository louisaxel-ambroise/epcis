using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v2_0.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v2_0.Communication;

[TestClass]
public class WhenFormattingAnEmptySubscriptionResult
{
    public QueryResponse Result = new("ExampleQueryName", "ASubscription", QueryData.Empty);
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
        Assert.AreEqual(3, Formatted.Elements().Count());
        Assert.AreEqual(Result.QueryName, Formatted.Element("queryName").Value);
        Assert.AreEqual(Result.SubscriptionId, Formatted.Element("subscriptionID").Value);
        Assert.IsNotNull(Formatted.Element("resultsBody"));
    }
}
