using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v1_2.Communication.Formatters;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAnEmptySubscriptionResult
{
    public PollResult Result = new("ExampleQueryName", "TestSubscription", new List<Event>());
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
