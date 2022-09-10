using FasTnT.Application.Commands;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Commands.Capture;
using FasTnT.Domain.Model;

namespace FasTnT.Application.Tests;

[TestClass]
public class WhenHandlingCaptureRequest
{
    readonly static EpcisContext Context = Tests.Context.TestContext.GetContext(nameof(WhenHandlingCaptureRequest));
    readonly static ICurrentUser UserContext = new TestCurrentUser();

    [TestMethod]
    public void ItShouldReturnACaptureResultAndStoreTheRequest()
    {
        var handler = new CaptureEpcisRequestCommandHandler(Context, UserContext);
        var request = new CaptureEpcisRequestCommand { Request = new Request { SchemaVersion = "1.0" } };
        var result = handler.Handle(request, default).Result;
            
        Assert.IsNotNull(result);
        Assert.AreEqual(1, Context.Requests.Count());
    }
}
