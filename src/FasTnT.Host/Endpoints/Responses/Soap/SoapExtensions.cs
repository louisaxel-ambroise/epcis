﻿using FasTnT.Domain.Exceptions;
using FasTnT.Host.Communication.Xml.Formatters;
using FasTnT.Host.Communication.Xml.Utils;
using System.Xml;
using System.Xml.XPath;

namespace FasTnT.Host.Endpoints.Responses.Soap;

public static class SoapExtensions
{
    private static readonly XmlWriterSettings _soapsettings = new() { Async = true, NamespaceHandling = NamespaceHandling.OmitDuplicates, Indent = false };

    public static async Task FormatSoap(this HttpResponse response, XElement element, CancellationToken cancellationToken)
    {
        response.ContentType = "application/xml";

        var body = new XElement(XName.Get("Body", Namespaces.SoapEnvelop), element);
        var envelope = new XElement(XName.Get("Envelope", Namespaces.SoapEnvelop), body);
        var xmlResponse = new XDocument(envelope);

        envelope.Add(
            new XAttribute(XNamespace.Xmlns + "soapenv", Namespaces.SoapEnvelop),
            new XAttribute(XNamespace.Xmlns + "epcisq", Namespaces.Query),
            new XAttribute(XNamespace.Xmlns + "xsd", Namespaces.XSD),
            new XAttribute(XNamespace.Xmlns + "xsi", Namespaces.XSI));

        await using var xmlWriter = XmlWriter.Create(response.Body, _soapsettings);
        await xmlResponse.WriteToAsync(xmlWriter, cancellationToken);
    }

    public static Task FormatSoapFault(this HttpResponse response, EpcisException error, CancellationToken cancellationToken)
    {
        var fault = new XElement(XName.Get("Fault", Namespaces.SoapEnvelop),
            new XElement("faultCode", "Server"),
            new XElement("faultstring", error.Message),
            new XElement("detail", XmlResponseFormatter.FormatError(error)));

        return response.FormatSoap(fault, cancellationToken);
    }

    public static async Task<XElement> ParseSoapEnvelope(this HttpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var document = await XDocument.LoadAsync(request.Body, LoadOptions.None, cancellationToken);
            var envelopBody = document.XPathSelectElement("SoapEnvelop:Envelope/SoapEnvelop:Body", Namespaces.Resolver);

            if (envelopBody == null || !envelopBody.HasElements)
            {
                return null;
            }

            return envelopBody.Elements().SingleOrDefault(x => x.Name.NamespaceName == Namespaces.Query);
        }
        catch
        {
            throw new EpcisException(ExceptionType.ValidationException, "Malformed or Invalid SOAP payload");
        }
    }

    public static XElement FormatFault(EpcisException exception)
    {
        return new(XName.Get("Fault", Namespaces.SoapEnvelop), new XElement("faultCode", "server"), new XElement("detail", XmlResponseFormatter.FormatError(exception)));
    }
}
