using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FasTnT.Formatter.Xml.Parsers
{
    public static class XmlEpcisDocumentParser
    {
        public static Request Parse(XElement root)
        {
            var request = new Request
            {
                CaptureDate = DateTime.UtcNow,
                DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value),
                SchemaVersion = root.Attribute("schemaVersion").Value
            };

            ParseHeaderIntoRequest(root.Element("EPCISHeader"), request);
            ParseBodyIntoRequest(root.Element("EPCISBody"), request);

            return request;
        }

        private static void ParseHeaderIntoRequest(XElement epcisHeader, Request request)
        {
            var masterData = epcisHeader?.XPathSelectElement("extension/EPCISMasterData/VocabularyList");

            if (masterData != default)
            {
                request.Masterdata.AddRange(XmlMasterdataParser.ParseMasterdata(masterData));
            }
        }

        private static void ParseBodyIntoRequest(XElement epcisBody, Request request)
        {
            var element = epcisBody.Elements().First();

            switch (element.Name.LocalName)
            {
                case "EventList":
                    request.Events = XmlEventParser.ParseEvents(element).ToList();
                    break;
                case "VocabularyList":
                    request.Masterdata = XmlMasterdataParser.ParseMasterdata(element).ToList();
                    break;
                default:
                    throw new EpcisException(ExceptionType.ValidationException, $"Invalid element: {element.Name.LocalName}");
            }
        }
    }
}
