using FasTnT.Application.Queries.GetVendorVersion;
using FasTnT.Domain;
using FasTnT.Domain.Queries.GetVendorVersion;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetVendorVersionQuery
{
    [TestMethod]
    public void ItShouldReturnTheCorrectVersion()
    {
        var handler = new GetVendorVersionQueryHandler();
        var result = handler.Handle(new GetVendorVersionQuery(), default).Result;
            
        Assert.IsInstanceOfType(result, typeof(GetVendorVersionResult));

        var vendorVersion = (GetVendorVersionResult)result;
        Assert.AreEqual(Constants.VendorVersion, vendorVersion.Version);
    }
}
