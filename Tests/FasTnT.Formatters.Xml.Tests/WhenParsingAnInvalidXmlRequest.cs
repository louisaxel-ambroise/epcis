using FasTnT.Formatter.Xml.Parsers;
using System.Reflection;

namespace FasTnT.Formatters.Xml.Tests;

[TestClass]
public class WhenParsingAnInvalidXmlRequest : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.Requests.InvalidRequest.xml";

    [TestMethod]
    public void ItShouldThrowAnException()
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);

        Assert.ThrowsException<AggregateException>(() => XmlDocumentParser.Instance.ParseAsync(manifest, default).Wait());
    }
}
