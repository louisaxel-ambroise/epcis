using FasTnT.Domain.Commands;
using FasTnT.Domain.Exceptions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Formatter.Xml.Parsers
{
    public static class CaptureRequestParser
    {
        public static async Task<CaptureEpcisRequestCommand> Parse(Stream input, CancellationToken cancellationToken)
        {
            var document = await XmlDocumentParser.Instance.ParseAsync(input, cancellationToken);
            var request = XmlEpcisDocumentParser.Parse(document.Root);

            return request != default
                    ? new CaptureEpcisRequestCommand { Request = request }
                    : throw new EpcisException(ExceptionType.ValidationException, $"Document with root '{document.Root.Name}' is not expected here.");
        }
    }
}
