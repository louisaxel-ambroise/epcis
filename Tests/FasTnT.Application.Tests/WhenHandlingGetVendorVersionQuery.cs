using FasTnT.Application.Queries;
using FasTnT.Domain;
using FasTnT.Domain.Queries;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetVendorVersionQuery
{
    [TestMethod]
    public void ItShouldReturnTheCorrectVersion()
    {
        var handler = new GetVendorVersionQueryHandler();
        var result = handler.Handle(new GetVendorVersionQuery(), default).Result;
            
        Assert.AreEqual(Constants.VendorVersion, result.Version);
    }
}
