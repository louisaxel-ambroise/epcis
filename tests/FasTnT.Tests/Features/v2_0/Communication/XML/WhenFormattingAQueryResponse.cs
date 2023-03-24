using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingAQueryResponse
{
    public QueryResponse Result = new("ExampleQueryName", new List<Event>
    {
        new Event
        {
            Type = EventType.ObjectEvent,
            Fields = new List<Field>{ new Field { Type = FieldType.Extension, Namespace = "customNamespace", Name = "test", TextValue = "value" } }
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

    [TestMethod]
    public void TheCustomNamespacesShouldBePrefixed()
    {
        var element = XElement.Parse(Formatted);

        Assert.IsNotNull(element.Attribute(XName.Get("ext0", XNamespace.Xmlns.NamespaceName)));
    }
}
