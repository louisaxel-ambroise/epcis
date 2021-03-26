using FasTnT.Domain.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace FasTnT.Formatter.Xml.Parsers
{
    public class XmlDocumentParser
    {
        public static XmlDocumentParser Instance { get; } = new XmlDocumentParser();
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
            var document = await XDocument.LoadAsync(input, LoadOptions.None, cancellationToken);
            document.Validate(_schema, (e, t) => { if (t.Exception != null) throw new EpcisException(ExceptionType.ValidationException, t.Exception.Message); });

            return document;
        }
    }
}
