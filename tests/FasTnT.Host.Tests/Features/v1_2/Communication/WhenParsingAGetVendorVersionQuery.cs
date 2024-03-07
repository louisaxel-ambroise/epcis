using FasTnT.Host.Communication.Xml.Parsers;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAGetVendorVersionQuery : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Queries.GetVendorVersion.xml";

    public object Query { get; set; }

    [TestInitialize]
    public void When()
    {
        Query = ParseResource(ResourceName).Parse();
    }

    [TestMethod]
    public void ItShouldReturnAGetStandardVersionObject()
    {
        Assert.IsInstanceOfType(Query, typeof(GetVendorVersion));
    }
}
