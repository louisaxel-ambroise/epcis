using FasTnT.Host.Features.v1_2.Communication;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

namespace FasTnT.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAGetVendorVersionQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Tests.Features.v1_2.Communication.Resources.Queries.GetVendorVersion.xml";

    public object Query { get; set; }

    [TestInitialize]
    public void When()
    {
        Query = XmlQueryParser.Parse(ParseResource(ResourceName).Root);
    }

    [TestMethod]
    public void ItShouldReturnAGetStandardVersionObject()
    {
        Assert.IsInstanceOfType(Query, typeof(GetVendorVersion));
    }
}
