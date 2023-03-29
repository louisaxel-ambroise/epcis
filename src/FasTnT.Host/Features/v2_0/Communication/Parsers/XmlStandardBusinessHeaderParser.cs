using FasTnT.Application.Domain.Model;
using FasTnT.Application.Domain.Model.Events;

namespace FasTnT.Host.Features.v2_0.Communication.Parsers;

internal static class XmlStandardBusinessHeaderParser
{
    const string Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader";
    public static StandardBusinessHeader ParseHeader(XElement sbdh)
    {
        var documentIdentification = sbdh.Element(XName.Get("DocumentIdentification", Namespace));

        return new()
        {
            Version = sbdh.Element(XName.Get("HeaderVersion", Namespace))?.Value,
            Standard = documentIdentification?.Element(XName.Get("Standard", Namespace))?.Value,
            TypeVersion = documentIdentification?.Element(XName.Get("TypeVersion", Namespace))?.Value,
            CreationDateTime = DateTime.TryParse(documentIdentification?.Element(XName.Get("CreationDateAndTime", Namespace))?.Value, null, DateTimeStyles.AdjustToUniversal, out DateTime result) ? result : default,
            InstanceIdentifier = documentIdentification?.Element(XName.Get("InstanceIdentifier", Namespace))?.Value,
            Type = documentIdentification?.Element(XName.Get("Type", Namespace))?.Value,
            ContactInformations = ParseContactInformations(sbdh)
        };
    }

    private static List<ContactInformation> ParseContactInformations(XElement sbdh)
    {
        var result = new List<ContactInformation>();

        var sender = sbdh.Element(XName.Get("Sender", Namespace));
        var receiver = sbdh.Element(XName.Get("Receiver", Namespace));

        if (sender != default)
        {
            result.Add(ParseContactInformation(sender, ContactInformationType.Sender));
        }
        if (receiver != default)
        {
            result.Add(ParseContactInformation(receiver, ContactInformationType.Receiver));
        }

        return result;
    }

    private static ContactInformation ParseContactInformation(XElement element, ContactInformationType type)
    {
        return new()
        {
            Type = type,
            Identifier = element.Element(XName.Get("Identifier", Namespace))?.Value,
            Contact = element.Element(XName.Get("Contact", Namespace))?.Value,
            ContactTypeIdentifier = element.Element(XName.Get("ContactTypeIdentifier", Namespace))?.Value,
            EmailAddress = element.Element(XName.Get("EmailAddress", Namespace))?.Value,
            FaxNumber = element.Element(XName.Get("FaxNumber", Namespace))?.Value,
            TelephoneNumber = element.Element(XName.Get("TelephoneNumber", Namespace))?.Value
        };
    }
}
