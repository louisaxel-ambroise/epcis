using FasTnT.Application.Queries.GetStandardVersion;
using FasTnT.Domain.Queries.GetStandardVersion;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.Application.Tests
{
    [TestClass]
    public class WhenHandlingGetStandardVersionQuery
    {
        [TestMethod]
        public void ItShouldReturnV1_2()
        {
            var handler = new GetStandardVersionQueryHandler();
            var result = handler.Handle(new GetStandardVersionQuery(), default).Result;
            
            Assert.AreEqual("1.2", result.Version);
        }
    }
}
