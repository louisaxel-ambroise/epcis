using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Host.Features.v2_0.Communication.Xml.Parsers;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenParsingARequestContainingAStandardBusinessHeader : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v2_0.Communication.Resources.Requests.Request_WithHeader.xml";

    public Request Request { get; set; }

    [TestInitialize]
    public void When()
    {
        Request = XmlEpcisDocumentParser.Parse(ParseResource(ResourceName));
    }

    [TestMethod]
    public void RequestShouldContainsOneEvent()
    {
        Assert.IsNotNull(Request.StandardBusinessHeader);
    }

    [TestMethod]
    public void TheHeaderShouldContainTheSender()
    {
        Assert.AreEqual(2, Request.StandardBusinessHeader.ContactInformations.Count);
        Assert.AreEqual(1, Request.StandardBusinessHeader.ContactInformations.Count(x => x.Type == ContactInformationType.Sender));
        Assert.AreEqual("Sending Party", Request.StandardBusinessHeader.ContactInformations.Single(x => x.Type == ContactInformationType.Sender).Identifier);
        Assert.AreEqual(1, Request.StandardBusinessHeader.ContactInformations.Count(x => x.Type == ContactInformationType.Receiver));
        Assert.AreEqual("Receiving Party", Request.StandardBusinessHeader.ContactInformations.Single(x => x.Type == ContactInformationType.Receiver).Identifier);
    }

    [TestMethod]
    public void TheHeaderDocumentIdentificationFieldsShouldBeParsed()
    {
        Assert.AreEqual("EPCglobal", Request.StandardBusinessHeader.Standard);
        Assert.AreEqual("1.2", Request.StandardBusinessHeader.TypeVersion);
        Assert.AreEqual("201812244444400001", Request.StandardBusinessHeader.InstanceIdentifier);
        Assert.AreEqual("Events", Request.StandardBusinessHeader.Type);
        Assert.AreEqual(new DateTime(2018, 12, 06, 12, 45, 20, DateTimeKind.Utc), Request.StandardBusinessHeader.CreationDateTime);

    }
}
