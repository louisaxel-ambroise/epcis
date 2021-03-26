using FasTnT.Application.Commands;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Formatter.Xml.Parsers
{
    public static class CaptureRequestParser
    {
        public static async Task<IBaseRequest> ParseEpcisDocumentAsync(Stream input, CancellationToken cancellationToken)
        {
            var document = await XmlDocumentParser.Instance.ParseAsync(input, cancellationToken);
            var request = XmlEpcisDocumentParser.Parse(document.Root);

            return request != default
                    ? new CaptureEpcisRequestCommand { Request = request }
                    : throw new Exception($"Document with root '{document.Root.Name}' is not expected here.");
        }
    }
}
