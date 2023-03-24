using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model.Masterdata;
using System.Text.Json;

namespace FasTnT.Application.Domain.Format.v2_0.Parsers;

public class JsonMasterdataParser
{
    private readonly JsonElement _element;
    private readonly Namespaces _extensions;

    private JsonMasterdataParser(JsonElement element, Namespaces extensions)
    {
        _element = element;
        _extensions = extensions;
    }

    public static JsonMasterdataParser Create(JsonElement element, Namespaces extensions)
    {
        if (element.TryGetProperty("@context", out var context))
        {
            extensions = extensions.Merge(Namespaces.Parse(context));
        }

        return new(element, extensions);
    }

    public IEnumerable<MasterData> Parse()
    {
        var type = _element.GetProperty("type").GetString();

        return _element.GetProperty("vocabularyElementList").EnumerateArray().Select(x => ParseVocabularyElement(x, type));
    }

    private MasterData ParseVocabularyElement(JsonElement element, string type)
    {
        var masterdata = new MasterData { Type = type };

        foreach (var property in element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "id":
                    masterdata.Id = property.Value.GetString(); break;
                case "attributes":
                    masterdata.Attributes = property.Value.EnumerateArray().Select(ParseVocabularyAttribute).ToList(); break;
                case "children":
                    masterdata.Children = property.Value.EnumerateArray().Select(x => new MasterDataChildren { ChildrenId = x.GetString() }).ToList(); break;
                default:
                    throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected field: {property.Name}");
            }
        }

        return masterdata;
    }

    private MasterDataAttribute ParseVocabularyAttribute(JsonElement element)
    {
        return new()
        {
            Id = element.GetProperty("id").GetString(),
            Value = element.GetProperty("attribute").ValueKind == JsonValueKind.Object ? string.Empty : element.GetProperty("attribute").GetString(),
            Fields = ParseFields(element.GetProperty("attribute"), null, null),
        };
    }

    private List<MasterDataField> ParseFields(JsonElement element, string parentName, string parentNamespace)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return new List<MasterDataField>();
        }

        var result = new List<MasterDataField>();

        foreach (var property in element.EnumerateObject())
        {
            var (ns, name) = _extensions.ParseName(property.Name);

            result.AddRange(ParseFields(property.Value, name, ns));
            result.Add(new MasterDataField
            {
                Value = property.Value.ValueKind == JsonValueKind.Object ? null : property.Value.GetString(),
                Name = name,
                Namespace = ns,
                ParentName = parentName,
                ParentNamespace = parentNamespace,
            });
        }

        return result;
    }
}
