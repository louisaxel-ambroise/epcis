using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FasTnT.IntegrationTests.v1_2;

public class XmlResponseExtensions
{
    public static T ParseSoap<T>(string content)
    {
        var soapResult = XDocument.Parse(content);
        var element = soapResult.Root.Element(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"))
            .Elements().FirstOrDefault();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(element.ToString()));
        stream.Seek(0, SeekOrigin.Begin);

        var result = (T)new XmlSerializer(typeof(T)).Deserialize(stream);

        return result;
    }
}