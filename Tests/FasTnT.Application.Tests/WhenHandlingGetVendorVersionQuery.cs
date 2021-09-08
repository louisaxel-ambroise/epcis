using FasTnT.Application.Queries.GetVendorVersion;
using FasTnT.Domain.Queries.GetVendorVersion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Application.Tests
{
    [TestClass]
    public class WhenHandlingGetVendorVersionQuery
    {
        [TestMethod]
        public void ItShouldReturnV0_5()
        {
            var handler = new GetVendorVersionQueryHandler();
            var result = handler.Handle(new GetVendorVersionQuery(), default).Result;
            
            Assert.AreEqual("0.5", result.Version);
        }
    }
}
