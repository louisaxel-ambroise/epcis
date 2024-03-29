using FasTnT.Domain.Model.Masterdata;

namespace FasTnT.Host.Communication.Xml.Parsers;

public class XmlMasterdataParser
{
    private int _index;

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
        return element?.Elements("id")?.Select(x => new MasterDataChildren { ChildrenId = x.Value })?.ToList() ?? [];
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

    private List<MasterDataField> ParseField(XElement element, int? parentIndex = null)
    {
        var result = new List<MasterDataField>
        {
            new()
            {
                Index = ++_index,
                Value = element.HasElements ? null : element.Value,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                ParentIndex = parentIndex
            }
        };

        if (element.HasElements)
        {
            result.AddRange(element.Elements().SelectMany(x => ParseField(x, _index)));
        }

        return result;
    }
}
