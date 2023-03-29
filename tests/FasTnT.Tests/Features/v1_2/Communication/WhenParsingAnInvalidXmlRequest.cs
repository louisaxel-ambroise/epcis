using FasTnT.Host.Features.v1_2.Communication.Parsers;
using System.Reflection;

namespace FasTnT.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAnInvalidXmlRequest : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Tests.Features.v1_2.Communication.Resources.Requests.InvalidRequest.xml";

    [TestMethod]
    public void ItShouldThrowAnException()
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);

        Assert.ThrowsException<AggregateException>(() => XmlDocumentParser.Instance.ParseAsync(manifest, default).Wait());
    }
}
