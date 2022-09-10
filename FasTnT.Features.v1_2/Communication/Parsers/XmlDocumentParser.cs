using FasTnT.Domain.Exceptions;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace FasTnT.Formatter.Xml.Parsers;

public class XmlDocumentParser
{
    public static XmlDocumentParser Instance { get; } = new();
    private readonly XmlSchemaSet _schema;

    private XmlDocumentParser()
    {
        _schema = new XmlSchemaSet();

        var assembly = Assembly.GetExecutingAssembly();
        var xsdFiles = assembly.GetManifestResourceNames().Where(x => x.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase));

        foreach (var file in xsdFiles.Select(assembly.GetManifestResourceStream).Select(XmlReader.Create))
        {
            _schema.Add(null, file);
        }

        _schema.Compile();
    }

    public async Task<XDocument> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var document = await LoadDocument(input, cancellationToken).ConfigureAwait(false);
        document.Validate(_schema, (_, t) =>
        {
            if (t.Exception != null)
            {
                throw new EpcisException(ExceptionType.ValidationException, t.Message);
            }
        });

        return document;
    }

    private static async Task<XDocument> LoadDocument(Stream input, CancellationToken cancellationToken)
    {
        try
        {
            return await XDocument.LoadAsync(input, LoadOptions.None, cancellationToken);
        }
        catch
        {
            throw new FormatException("XML is invalid");
        }
    }
}
