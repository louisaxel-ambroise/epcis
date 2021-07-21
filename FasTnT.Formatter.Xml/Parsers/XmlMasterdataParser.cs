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
            var list = new List<MasterData>();

            foreach (var element in root.Elements("Vocabulary"))
            {
                list.AddRange(ParseVocabulary(element));
            }

            return list;
        }

        private static IEnumerable<MasterData> ParseVocabulary(XElement element)
        {
            var type = element.Attribute("type").Value;

            foreach (var vocElement in element.Element("VocabularyElementList")?.Elements("VocabularyElement"))
            {
                yield return ParseVocabularyElement(vocElement, type);
            }
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
            var attribute = new MasterDataAttribute
            {
                Id = element.Attribute("id").Value,
                Value = element.HasElements ? string.Empty : element.Value
            };

            attribute.Fields.AddRange(element.Elements().Select(ParseField));

            return attribute;
        }

        private static MasterDataField ParseField(XElement element)
        {
            var field = new MasterDataField
            {
                Value = element.HasElements ? element.Value : null,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName
            };

            field.Children.AddRange(element.Elements().Select(ParseField));

            return field;
        }
    }
}
