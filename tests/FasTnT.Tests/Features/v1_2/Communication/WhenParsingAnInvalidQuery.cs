using FasTnT.Host.Features.v1_2.Communication;

namespace FasTnT.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAnInvalidQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Tests.Features.v1_2.Communication.Resources.Queries.InvalidQuery.xml";

    [TestMethod]
    public void ItShouldReturnAGetQueryNamesObject()
    {
        Assert.ThrowsException<AggregateException>(() => XmlQueryParser.Parse(ParseResource(ResourceName).Root));
    }
}
