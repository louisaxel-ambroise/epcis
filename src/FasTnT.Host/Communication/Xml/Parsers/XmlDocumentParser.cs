using FasTnT.Domain.Exceptions;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace FasTnT.Host.Communication.Xml.Parsers;

public class XmlDocumentParser
{
    public static XmlDocumentParser Instance { get; } = new();
    private readonly XmlSchemaSet 
        _v1schema = CompileSchema("v1"), 
        _v2schema = CompileSchema("v2");

    public async Task<XmlEpcisDocumentParser> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var document = await LoadDocument(input, cancellationToken).ConfigureAwait(false);

        return document.Root?.Attribute("schemaVersion")?.Value switch
        {
            "1.0" or "1.1" or "1.2" => ParseDocument(document, _v1schema, new XmlV1EventParser()),
            "2.0" => ParseDocument(document, _v2schema, new XmlV2EventParser()),
            _ => throw new EpcisException(ExceptionType.ValidationException, "Unsupported EPCIS schemaVersion")
        };
    }

    private static XmlEpcisDocumentParser ParseDocument(XDocument document, XmlSchemaSet schema, XmlEventParser parser)
    {
        document.Validate(schema, (_, t) =>
        {
            if (t.Exception is not null)
            {
                throw new EpcisException(ExceptionType.ValidationException, t.Message);
            }
        });

        return new(document.Root, parser);
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

    private static XmlSchemaSet CompileSchema(string version)
    {
        var schema = new XmlSchemaSet();
        var assembly = Assembly.GetExecutingAssembly();
        var sharedFiles = assembly.GetManifestResourceNames().Where(x => x.StartsWith("FasTnT.Host.Communication.Xml.Schemas.shared.") && x.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase));

        foreach (var file in sharedFiles.Select(assembly.GetManifestResourceStream).Select(XmlReader.Create))
        {
            schema.Add(null, file);
        }

        var v1Files = assembly.GetManifestResourceNames().Where(x => x.StartsWith($"FasTnT.Host.Communication.Xml.Schemas.{version}.") && x.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase));
        foreach (var file in v1Files.Select(assembly.GetManifestResourceStream).Select(XmlReader.Create))
        {
            schema.Add(null, file);
        }

        schema.Compile();

        return schema;
    }
}
