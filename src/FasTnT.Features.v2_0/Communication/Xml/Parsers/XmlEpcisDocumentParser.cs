using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model;

namespace FasTnT.Features.v2_0.Communication.Xml.Parsers;

public static class XmlEpcisDocumentParser
{
    public static Request Parse(XElement root)
    {
        var request = new Request
        {
            CaptureDate = DateTime.UtcNow,
            DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value),
            SchemaVersion = root.Attribute("schemaVersion").Value
        };

        ParseBodyIntoRequest(root.Element("EPCISBody"), request);

        return request;
    }

    private static void ParseBodyIntoRequest(XElement epcisBody, Request request)
    {
        var element = epcisBody.Elements().First();

        request.Events = element.Name.LocalName == "EventList" 
            ? XmlEventParser.ParseEvents(element).ToList()
            : throw new EpcisException(ExceptionType.ValidationException, $"Invalid element: {element.Name.LocalName}");
    }
}
