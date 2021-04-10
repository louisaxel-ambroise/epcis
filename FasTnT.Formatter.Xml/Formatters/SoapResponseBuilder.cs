using FasTnT.Formatter.Xml.Utils;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml
{
    public class SoapResponseBuilder {
        internal static string WrapSoap11(XElement response)
        {
            return new XDocument(new XElement(XName.Get("Envelope", Namespaces.SoapEnvelop),
                new XAttribute(XNamespace.Xmlns + "soapenv", Namespaces.SoapEnvelop),
                new XAttribute(XNamespace.Xmlns + "epcisq", Namespaces.Query),
                new XElement(XName.Get("Body", Namespaces.SoapEnvelop), response)
            )).ToString(SaveOptions.DisableFormatting);
        }
    }
}
