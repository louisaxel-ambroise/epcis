using FasTnT.Host.Communication.Xml.Parsers;
using System.Reflection;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingAnInvalidXmlRequest : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.Requests.InvalidRequest.xml";

    [TestMethod]
    public void ItShouldThrowAnException()
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);

        Assert.ThrowsAsync<AggregateException>(() => XmlDocumentParser.Instance.ParseAsync(manifest, default));
    }
}
