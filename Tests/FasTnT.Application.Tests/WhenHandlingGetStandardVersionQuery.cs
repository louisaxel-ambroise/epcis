using FasTnT.Application.Queries;
using FasTnT.Domain.Queries;

namespace FasTnT.Application.Tests;

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
