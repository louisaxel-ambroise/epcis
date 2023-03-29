using FasTnT.Host.Features.v2_0.Communication.Formatters;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingAQueryResponseVithVocabulary
{
    public QueryResponse Result = new("ExampleQueryName", new List<MasterData>
    {
        new MasterData
        {
            Id = "md0.1230",
            Type = "readPoint",
            Attributes = new List<MasterDataAttribute>{ new MasterDataAttribute { Id = "test", Value = "xyz" } }
        }
    });
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
