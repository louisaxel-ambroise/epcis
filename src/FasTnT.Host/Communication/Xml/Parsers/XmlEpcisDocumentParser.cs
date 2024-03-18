using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Communication.Xml.Utils;
using System.Xml.XPath;

namespace FasTnT.Host.Communication.Xml.Parsers;

public class XmlEpcisDocumentParser(XElement root, XmlEventParser eventParser)
{
    private Request _request;

    public Request Parse()
    {
        _request = new Request
        {
            DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value, null, DateTimeStyles.AdjustToUniversal),
            SchemaVersion = root.Attribute("schemaVersion").Value
        };

        ParseHeader(root.Element("EPCISHeader"));
        ParseBody(root.Element("EPCISBody"));

        return _request;
    }

    private void ParseHeader(XElement epcisHeader)
    {
        var sbdh = epcisHeader?.Element(XName.Get("StandardBusinessDocumentHeader", Namespaces.SBDH));
        var masterData = epcisHeader?.XPathSelectElement("extension/EPCISMasterData/VocabularyList");

        if (sbdh != default)
        {
            _request.StandardBusinessHeader = XmlStandardBusinessHeaderParser.ParseHeader(sbdh);
        }
        if (masterData != default)
        {
            _request.Masterdata.AddRange(XmlMasterdataParser.ParseMasterdata(masterData));
        }
    }

    private void ParseBody(XElement epcisBody)
    {
        var element = epcisBody.Elements().First();

        switch (element.Name.LocalName)
        {
            case "QueryResults":
                ParseCallbackResult(element);
                break;
            case "EventList":
                _request.Events = eventParser.ParseEvents(element).ToList();
                break;
            case "VocabularyList":
                _request.Masterdata = XmlMasterdataParser.ParseMasterdata(element).ToList();
                break;
            default:
                throw new EpcisException(ExceptionType.ValidationException, $"Invalid element: {element.Name.LocalName}");
        }
    }

    private void ParseCallbackResult(XElement queryResults)
    {
        var subscriptionId = queryResults.Element("subscriptionID")?.Value;
        var eventList = queryResults.Element("resultsBody").Element("EventList");

        _request.Events.AddRange(eventParser.ParseEvents(eventList));
        _request.SubscriptionCallback = new SubscriptionCallback
        {
            CallbackType = QueryCallbackType.Success,
            SubscriptionId = subscriptionId
        };
    }
}
