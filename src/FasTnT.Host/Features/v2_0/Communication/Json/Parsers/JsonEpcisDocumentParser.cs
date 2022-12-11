using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using System.Text.Json;

namespace FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

public static class JsonEpcisDocumentParser
{
    public static Request Parse(JsonDocument document, Namespaces extensions)
    {
        if (document.RootElement.TryGetProperty("@context", out JsonElement context))
        {
            extensions = extensions.Merge(Namespaces.Parse(context));
        }

        var request = new Request();

        // TODO: parse sender/receiver that is in root element in JSON format.
        foreach (var property in document.RootElement.EnumerateObject())
        {
            switch (property.Name)
            {
                case "schemaVersion":
                    request.SchemaVersion = property.Value.GetString(); break;
                case "creationDate":
                    request.DocumentTime = property.Value.GetDateTime(); break;
                case "epcisBody":
                    request.Events = ParseEvents(property.Value, extensions); break;
                case "epcisHeader":
                    request.Masterdata = ParseMasterdata(property.Value, extensions); break;
                case "id" or "type" or "@context": break; // Ignore these fields - they are either already parsed or irrelevant
                default:
                    throw new EpcisException(ExceptionType.ImplementationException, $"Unknown property type: '{property.Name}'");
            }
        }

        return request;
    }

    public static List<Event> ParseEvents(JsonElement element, Namespaces extensions)
    {
        return element.GetProperty("eventList")
            .EnumerateArray()
            .Select(x => JsonEventParser.Create(x, extensions).Parse())
            .ToList();
    }

    public static List<MasterData> ParseMasterdata(JsonElement element, Namespaces extensions)
    {
        var masterdataList = new List<MasterData>();

        if (element.TryGetProperty("epcisMasterData", out var masterdata))
        {
            masterdataList.AddRange(masterdata.GetProperty("vocabularyList")
                .EnumerateArray()
                .SelectMany(x => JsonMasterdataParser.Create(x, extensions).Parse()));
        }

        return masterdataList;
    }
}
