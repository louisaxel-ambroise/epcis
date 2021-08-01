using FasTnT.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml
{
    public static class XmlMasterdataParser
    {
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
            return new()
            {
                Type = type,
                Id = element.Attribute("id").Value,
                Attributes = element.Elements("attribute").Select(ParseAttribute).ToList(),
                Children = ParseChildren(element.Element("children"))
            };
        }

        private static List<string> ParseChildren(XElement element)
        {
            return element?.Elements("id")?.Select(x => x.Value)?.ToList() ?? new();
        }

        private static MasterDataAttribute ParseAttribute(XElement element)
        {
            return new()
            {
                Id = element.Attribute("id").Value,
                Value = element.HasElements ? string.Empty : element.Value,
                Fields = element.Elements().Select(ParseField).ToList()
            };
        }

        private static MasterDataField ParseField(XElement element)
        {
            return new()
            {
                Value = element.HasElements ? element.Value : null,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                Children = element.Elements().Select(ParseField).ToList()
            };
        }
    }
}
