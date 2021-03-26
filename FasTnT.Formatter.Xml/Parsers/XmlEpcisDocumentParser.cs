using FasTnT.Domain.Model;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml.Parsers
{
    public static class XmlEpcisDocumentParser
    {
        public static Request Parse(XElement root)
        {
            var request = new Request
            {
                DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value),
                SchemaVersion = root.Attribute("schemaVersion").Value
            };

            ParseBodyIntoRequest(root.Element("EPCISBody"), request);

            return request;
        }

        private static void ParseBodyIntoRequest(XElement epcisBody, Request request)
        {
            var element = epcisBody.Elements().First();

            switch (element.Name.LocalName)
            {
                case "EventList":
                    request.Events.AddRange(XmlEventParser.ParseEvents(element)); break;
            }
        }
    }
}
