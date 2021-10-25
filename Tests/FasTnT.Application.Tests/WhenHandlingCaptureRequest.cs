using FasTnT.Application.Commands;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Tests.Context;
using FasTnT.Domain.Commands.Capture;
using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FasTnT.Application.Tests
{
    [TestClass]
    public class WhenHandlingCaptureRequest
    {
        readonly static EpcisContext Context = Tests.Context.TestContext.GetContext(nameof(Tests.WhenHandlingCaptureRequest));
        readonly static ICurrentUser UserContext = new TestCurrentUser();

        [TestMethod]
        public void ItShouldReturnACaptureResultAndStoreTheRequest()
        {
            var handler = new CaptureEpcisRequestCommandHandler(Context, UserContext);
            var request = new CaptureEpcisRequestCommand { Request = new Request { } };
            var result = handler.Handle(request, default).Result;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(1, Context.Requests.Count());
        }
    }
}
