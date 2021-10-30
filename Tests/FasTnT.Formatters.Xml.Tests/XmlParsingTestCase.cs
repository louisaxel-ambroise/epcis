using FasTnT.Formatter.Xml.Parsers;
using System.Reflection;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Tests;

public abstract class XmlParsingTestCase
{
    protected static XDocument ParseResource(string resourceName)
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using var resourceStream = XmlDocumentParser.Instance.ParseAsync(manifest, default);
         
        return resourceStream.Result;
    }
}
