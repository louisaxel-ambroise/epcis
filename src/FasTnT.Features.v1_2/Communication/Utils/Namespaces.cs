using System.Xml;

namespace FasTnT.Features.v1_2.Communication.Utils;

public static class Namespaces
{
    public static XmlNamespaceManager Resolver { get; } = new(new NameTable());

    static Namespaces()
    {
        Resolver.AddNamespace(nameof(SoapEnvelop), SoapEnvelop);
        Resolver.AddNamespace(nameof(Query), Query);
        Resolver.AddNamespace(nameof(Capture), Capture);
        Resolver.AddNamespace(nameof(MasterData), MasterData);
        Resolver.AddNamespace(nameof(SBDH), SBDH);
    }

    public static string SoapEnvelop => "http://schemas.xmlsoap.org/soap/envelope/";
    public static string Query => "urn:epcglobal:epcis-query:xsd:1";
    public static string Capture => "urn:epcglobal:epcis:xsd:1";
    public static string MasterData => "urn:epcglobal:epcis-masterdata:xsd:1";
    public static string SBDH => "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader";
    public static string XSD => "http://www.w3.org/2001/XMLSchema-instance";
    public static string XSI => "http://www.w3.org/2001/XMLSchema";
}
