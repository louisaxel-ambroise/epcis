using FasTnT.Application.Queries;
using FasTnT.Domain.Queries;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingGetVendorVersionQuery
{
    [TestMethod]
    public void ItShouldReturnV0_5()
    {
        var handler = new GetVendorVersionQueryHandler();
        var result = handler.Handle(new GetVendorVersionQuery(), default).Result;
            
        Assert.AreEqual("1.0.0", result.Version);
    }
}
