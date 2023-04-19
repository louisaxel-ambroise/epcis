using FasTnT.Domain.Model.Masterdata;

namespace FasTnT.Host.Features.v1_2.Communication.Parsers;

public class XmlMasterdataParser
{
    private int _index = 0;

    public static IEnumerable<MasterData> ParseMasterdata(XElement root)
    {
        return root.Elements("Vocabulary").SelectMany(ParseVocabulary);
    }

    private static IEnumerable<MasterData> ParseVocabulary(XElement element)
    {
        var type = element.Attribute("type").Value;

        return element
            .Element("VocabularyElementList")
            ?.Elements("VocabularyElement")
            ?.Select(x => ParseVocabularyElement(x, type));
    }

    private static MasterData ParseVocabularyElement(XElement element, string type)
    {
        var parser = new XmlMasterdataParser();

        return new()
        {
            Type = type,
            Id = element.Attribute("id").Value,
            Attributes = element.Elements("attribute").Select(parser.ParseVocabularyAttribute).ToList(),
            Children = ParseChildren(element.Element("children"))
        };
    }

    private static List<MasterDataChildren> ParseChildren(XElement element)
    {
        return element?.Elements("id")?.Select(x => new MasterDataChildren { ChildrenId = x.Value })?.ToList() ?? new();
    }

    private MasterDataAttribute ParseVocabularyAttribute(XElement element)
    {
        return new()
        {
            Index = ++_index,
            Id = element.Attribute("id").Value,
            Value = element.HasElements ? string.Empty : element.Value,
            Fields = element.Elements().SelectMany(x => ParseField(x)).ToList()
        };
    }

    private IEnumerable<MasterDataField> ParseField(XElement element, int? parentIndex = null)
    {
        var result = new List<MasterDataField>
        {
            new()
            {
                Index = ++_index,
                Value = element.HasElements ? null : element.Value,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                ParentIndex = parentIndex,
            }
        };

        if (element.HasElements)
        {
            result.AddRange(element.Elements().SelectMany(x => ParseField(x, _index)));
        }

        return result;
    }
}
