using FasTnT.Host.Features.v2_0.Communication.Xml.Parsers;
using System.Reflection;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

public abstract class XmlParsingTestCase
{
    protected static XElement ParseResource(string resourceName)
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using var resourceStream = XmlDocumentParser.Instance.ParseAsync(manifest, default);

        return resourceStream.Result.Root;
    }
}
