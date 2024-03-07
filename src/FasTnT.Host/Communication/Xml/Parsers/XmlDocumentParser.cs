using FasTnT.Domain.Exceptions;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace FasTnT.Host.Communication.Xml.Parsers;

public class XmlDocumentParser
{
    public static XmlDocumentParser Instance { get; } = new();
    private readonly XmlSchemaSet _schema;

    private XmlDocumentParser()
    {
        _schema = new XmlSchemaSet();

        var assembly = Assembly.GetExecutingAssembly();
        var xsdFiles = assembly.GetManifestResourceNames().Where(x => x.StartsWith("FasTnT.Host.Features.v2_0.") && x.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase));

        foreach (var file in xsdFiles.Select(assembly.GetManifestResourceStream).Select(XmlReader.Create))
        {
            _schema.Add(null, file);
        }

        _schema.Compile();
    }

    public async Task<XmlEpcisDocumentParser> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var document = await LoadDocument(input, cancellationToken).ConfigureAwait(false);

        // TODO: validate using the correct schemas.
        return document.Root?.Attribute("schemaVersion")?.Value switch
        {
            "1.0" or "1.1" or "1.2" => new (document.Root, new XmlV1EventParser()),
            "2.0" => new (document.Root, new XmlV2EventParser()),
            _ => throw new EpcisException(ExceptionType.ValidationException, "Unsupported EPCIS schemaVersion")
        };
    }

    private static async Task<XDocument> LoadDocument(Stream input, CancellationToken cancellationToken)
    {
        try
        {
            return await XDocument.LoadAsync(input, LoadOptions.None, cancellationToken);
        }
        catch
        {
            throw new EpcisException(ExceptionType.ValidationException, "XML is invalid");
        }
    }
}
