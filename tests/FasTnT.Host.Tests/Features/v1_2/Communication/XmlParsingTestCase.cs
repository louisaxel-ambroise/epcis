using FasTnT.Host.Communication.Xml.Parsers;
using FasTnT.Host.Endpoints.Responses.Soap;
using System.Reflection;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

public abstract class XmlParsingTestCase
{
    protected static XmlEpcisDocumentParser ParseResource(string resourceName)
    {
        return new XmlEpcisDocumentParser(ParseXml(resourceName), new XmlV1EventParser());
    }

    protected static XElement ParseXml(string resourceName)
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        
        return XDocument.Load(manifest).Root;
    }
}
