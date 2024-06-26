﻿using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;
using System.Xml.XPath;

namespace FasTnT.Host.Features.v2_0.Communication.Xml.Parsers;

public static class XmlEpcisDocumentParser
{
    public static Request Parse(XElement root)
    {
        var request = new Request
        {
            DocumentTime = UtcDateTime.Parse(root.Attribute("creationDate").Value),
            SchemaVersion = root.Attribute("schemaVersion").Value
        };

        ParseHeaderIntoRequest(root.Element("EPCISHeader"), request);
        ParseBodyIntoRequest(root.Element("EPCISBody"), request);

        return request;
    }

    private static void ParseHeaderIntoRequest(XElement epcisHeader, Request request)
    {
        var sbdh = epcisHeader?.Element(XName.Get("StandardBusinessDocumentHeader", "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader"));
        var masterData = epcisHeader?.XPathSelectElement("extension/EPCISMasterData/VocabularyList");

        if (sbdh != default)
        {
            request.StandardBusinessHeader = XmlStandardBusinessHeaderParser.ParseHeader(sbdh);
        }
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
            case "QueryResults":
                ParseCallbackResult(element, request);
                break;
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

    private static void ParseCallbackResult(XElement queryResults, Request request)
    {
        var subscriptionId = queryResults.Element("subscriptionID")?.Value;
        var eventList = queryResults.Element("resultsBody").Element("EventList");

        request.Events.AddRange(XmlEventParser.ParseEvents(eventList));
        request.SubscriptionCallback = new SubscriptionCallback
        {
            CallbackType = QueryCallbackType.Success,
            SubscriptionId = subscriptionId
        };
    }
}
