using FasTnT.Features.v1_2.Communication.Parsers;

namespace FasTnT.Features.v1_2.Tests;

[TestClass]
public class WhenParsingAnInvalidQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Features.v1_2.Tests.Resources.Queries.InvalidQuery.xml";

    [TestMethod]
    public void ItShouldReturnAGetQueryNamesObject()
    {
        Assert.ThrowsException<AggregateException>(() => XmlQueryParser.Parse(ParseResource(ResourceName).Root));
    }
}
