using FasTnT.Domain.Model.Masterdata;

namespace FasTnT.Host.Communication.Xml.Formatters;

public static class XmlMasterdataFormatter
{
    internal static IEnumerable<XElement> FormatMasterData(IList<MasterData> masterdataList)
    {
        var vocabularies = masterdataList.GroupBy(md => md.Type);

        return vocabularies.Select(FormatVocabulary);
    }

    private static XElement FormatVocabulary(IGrouping<string, MasterData> group)
    {
        var elements = new XElement("VocabularyElementList", group.Select(FormatVocabularyElement));

        return new XElement("Vocabulary", new XAttribute("type", group.Key), elements);
    }

    private static XElement FormatVocabularyElement(MasterData masterData)
    {
        var attributes = masterData.Attributes.Select(FormatVocabularyAttribute);
        var children = FormatVocabularyElementChildren(masterData.Children);

        return new XElement("VocabularyElement", new XAttribute("id", masterData.Id), attributes, children);
    }

    private static XElement FormatVocabularyElementChildren(List<MasterDataChildren> children)
    {
        return children.Count > 0 ? new XElement("children", children.Select(x => new XElement("id", x.ChildrenId))) : null;
    }

    private static XElement FormatVocabularyAttribute(MasterDataAttribute attribute)
    {
        object value = attribute.Fields.Count > 0 ? FormatFields(attribute.Fields) : attribute.Value;
        return new XElement("attribute", new XAttribute("id", attribute.Id), value);
    }

    private static List<XElement> FormatFields(List<MasterDataField> fields, int? parentIndex = null)
    {
        var formatted = new List<XElement>();

        foreach (var field in fields.Where(x => x.ParentIndex == parentIndex))
        {
            object value = fields.Any(f => f.ParentIndex == field.Index)
                ? FormatFields(fields, field.Index)
                : field.Value;

            formatted.Add(new XElement(XName.Get(field.Name, field.Namespace), value));
        }

        return formatted;
    }
}
