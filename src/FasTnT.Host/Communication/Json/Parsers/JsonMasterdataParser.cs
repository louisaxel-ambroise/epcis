using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Masterdata;
using System.Text.Json;

namespace FasTnT.Host.Communication.Json.Parsers;

public class JsonMasterdataParser
{
    private readonly JsonElement _element;
    private readonly Namespaces _extensions;
    private int _index;

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

        return _element.GetProperty("vocabularyElementList").EnumerateArray().Select((x, i) => ParseVocabularyElement(x, i, type));
    }

    private MasterData ParseVocabularyElement(JsonElement element, int index, string type)
    {
        var masterdata = new MasterData { Type = type, Index = index-1 };

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

    private MasterDataAttribute ParseVocabularyAttribute(JsonElement element, int index)
    {
        return new()
        {
            Id = element.GetProperty("id").GetString(),
            Index = index,
            Value = element.GetProperty("attribute").ValueKind == JsonValueKind.Object ? string.Empty : element.GetProperty("attribute").GetString(),
            Fields = ParseFields(element.GetProperty("attribute")),
        };
    }

    private List<MasterDataField> ParseFields(JsonElement element, int? parentIndex = null)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return [];
        }

        var result = new List<MasterDataField>();

        foreach (var property in element.EnumerateObject())
        {
            var (ns, name) = _extensions.ParseName(property.Name);

            result.Add(new MasterDataField
            {
                Index = ++_index,
                Value = property.Value.ValueKind == JsonValueKind.Object ? null : property.Value.GetString(),
                Name = name,
                Namespace = ns,
                ParentIndex = parentIndex
            });

            result.AddRange(ParseFields(property.Value, _index));
        }

        return result;
    }
}
