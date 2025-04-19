using FasTnT.Domain.Model.Masterdata;

namespace FasTnT.Host.Communication.Xml.Parsers;

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
            Index = ++_index,
            Attributes = element.Elements("attribute").Select(ParseVocabularyAttribute).ToList(),
            Children = ParseChildren(element.Element("children"))
        };
    }

    internal static List<MasterDataChildren> ParseChildren(XElement element)
    {
        return element?.Elements("id")?.Select(x => new MasterDataChildren { ChildrenId = x.Value })?.ToList() ?? [];
    }

    internal static MasterDataAttribute ParseVocabularyAttribute(XElement element, int index)
    {
        var parser = new XmlMasterdataFieldParser();

        return new()
        {
            Id = element.Attribute("id").Value,
            Index = index,
            Value = element.HasElements ? string.Empty : element.Value,
            Fields = element.Elements().SelectMany(x => parser.ParseField(x)).ToList()
        };
    }
}

internal class XmlMasterdataFieldParser
{
    private int _index;

    internal List<MasterDataField> ParseField(XElement element, int? parentIndex = null)
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