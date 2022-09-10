using FasTnT.Domain.Model;

namespace FasTnT.Formatter.Xml.Formatters;

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
        return children.Any() ? new XElement("children", children.Select(x => new XElement("id", x.ChildrenId))) : null;
    }

    private static XElement FormatVocabularyAttribute(MasterDataAttribute attribute)
    {
        var value = attribute.Fields.Any() ? FormatFields(attribute.Fields) : attribute.Value;
        return new XElement("attribute", new XAttribute("id", attribute.Id), value);
    }

    private static object FormatFields(List<MasterDataField> fields, string parentName = null, string parentNamespace = null)
    {
        var formatted = new List<XElement>();

        foreach(var field in fields.Where(x => x.ParentName == parentName && x.ParentNamespace == parentNamespace))
        {
            var value = fields.Any(f => f.ParentName == field.Name && f.ParentNamespace == field.Namespace)
                ? FormatFields(fields, field.Name, field.Namespace)
                : field.Value;

            formatted.Add(new XElement(XName.Get(field.Name, field.Namespace), value));
        }

        return formatted;
    }
}
