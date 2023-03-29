using FasTnT.Host.Features.v1_2.Communication;
using System.Reflection;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v1_2.Communication;

public abstract class XmlParsingTestCase
{
    protected static XDocument ParseResource(string resourceName)
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using var resourceStream = XmlDocumentParser.Instance.ParseAsync(manifest, default);

        return resourceStream.Result;
    }
}
