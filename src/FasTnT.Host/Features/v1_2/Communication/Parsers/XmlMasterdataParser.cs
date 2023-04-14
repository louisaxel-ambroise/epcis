using FasTnT.Domain.Model.Masterdata;

namespace FasTnT.Host.Features.v1_2.Communication.Parsers;

public class XmlMasterdataParser
{
    private int _index;

    public static IEnumerable<MasterData> ParseMasterdata(XElement root)
    {
        var parser = new XmlMasterdataParser();

        return root.Elements("Vocabulary").SelectMany(parser.ParseVocabulary);
    }

    private IEnumerable<MasterData> ParseVocabulary(XElement element)
    {
        var type = element.Attribute("type").Value;

        return element
            .Element("VocabularyElementList")
            ?.Elements("VocabularyElement")
            ?.Select(x => ParseVocabularyElement(x, type));
    }

    private MasterData ParseVocabularyElement(XElement element, string type)
    {
        return new()
        {
            Type = type,
            Id = element.Attribute("id").Value,
            Attributes = element.Elements("attribute").Select(ParseVocabularyAttribute).ToList(),
            Children = ParseChildren(element.Element("children"))
        };
    }

    private List<MasterDataChildren> ParseChildren(XElement element)
    {
        return element?.Elements("id")?.Select(x => new MasterDataChildren { ChildrenId = x.Value })?.ToList() ?? new();
    }

    private MasterDataAttribute ParseVocabularyAttribute(XElement element)
    {
        return new()
        {
            Id = element.Attribute("id").Value,
            Index = ++_index,
            Value = element.HasElements ? string.Empty : element.Value,
            Fields = element.Elements().SelectMany(x => ParseField(x)).ToList()
        };
    }

    private IEnumerable<MasterDataField> ParseField(XElement element, XName parentName = default)
    {
        var result = new List<MasterDataField>
        {
            new()
            {
                Value = element.HasElements ? null : element.Value,
                Index = ++_index,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                ParentName = parentName?.LocalName,
                ParentNamespace = parentName?.NamespaceName,
            }
        };

        if (element.HasElements)
        {
            result.AddRange(element.Elements().SelectMany(x => ParseField(x, element.Name)));
        }

        return result;
    }
}
