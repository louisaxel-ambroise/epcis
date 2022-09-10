using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Features.v1_2.Communication.Parsers;

namespace FasTnT.Features.v1_2.Tests;

[TestClass]
public class WhenParsingASuccessfulCallback : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Features.v1_2.Tests.Resources.CallbackRequests.SuccessfulRequest.xml";

    public Request Request { get; set; }

    [TestInitialize]
    public void When()
    {
        Request = XmlEpcisDocumentParser.Parse(ParseResource(ResourceName).Root);
    }

    [TestMethod]
    public void RequestShouldNotSpecifyUser()
    {
        Assert.IsNull(Request.User);
        Assert.AreEqual(default, Request.UserId);
    }

    [TestMethod]
    public void RequestShouldContainASubscriptionCallback()
    {
        Assert.IsNotNull(Request.SubscriptionCallback, "Request should contain a subscription callback");
    }

    [TestMethod]
    public void RequestShouldNotContainEventOrMasterdata()
    {
        Assert.IsTrue(Request.Events.Count == 0, "Request should not contain any event");
        Assert.IsTrue(Request.Masterdata.Count == 0, "Request should not contain any masterdata");
    }

    [TestMethod]
    public void SubscriptionCallbackTypeShouldBeSuccess()
    {
        Assert.AreEqual(QueryCallbackType.Success, Request.SubscriptionCallback.CallbackType);
    }

    [TestMethod]
    public void SubscriptionCallbackReasonShouldBeNull()
    {
        Assert.AreEqual(null, Request.SubscriptionCallback.Reason);
    }

    [TestMethod]
    public void SubscriptionIdShouldMatchTheRequestValue()
    {
        Assert.AreEqual("SubscriptionID", Request.SubscriptionCallback.SubscriptionId);
    }
}
