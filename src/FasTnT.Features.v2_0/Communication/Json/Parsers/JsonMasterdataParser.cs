using FasTnT.Domain.Model.Events;
using System.Text.Json;

namespace FasTnT.Features.v2_0.Communication.Json.Parsers;

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
                    masterdata.Children = element.EnumerateArray().Select(x => new MasterDataChildren { ChildrenId = x.GetString() }).ToList(); break;
                default: 
                    throw new NotImplementedException();
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
            var fieldName = ParseName(property.Name);

            result.AddRange(ParseFields(property.Value, fieldName.Name, fieldName.Namespace));
            result.Add(new MasterDataField
            {
                Value = property.Value.ValueKind == JsonValueKind.Object ? null : property.Value.GetString(),
                Name = fieldName.Name,
                Namespace = fieldName.Namespace,
                ParentName = parentName,
                ParentNamespace = parentNamespace,
            });
        }

        return result;
    }

    private (string Namespace, string Name) ParseName(string name)
    {
        var parts = name.Split(':', 2);

        return (_extensions[parts[0]], parts[1]);
    }
}
