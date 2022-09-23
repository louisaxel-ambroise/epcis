using FasTnT.Features.v1_2.Communication.Parsers;
using System.Reflection;

namespace FasTnT.Features.v1_2.Tests;

[TestClass]
public class WhenParsingAnInvalidXmlRequest : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Features.v1_2.Tests.Resources.Requests.InvalidRequest.xml";

    [TestMethod]
    public void ItShouldThrowAnException()
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);

        Assert.ThrowsException<AggregateException>(() => XmlDocumentParser.Instance.ParseAsync(manifest, default).Wait());
    }
}
