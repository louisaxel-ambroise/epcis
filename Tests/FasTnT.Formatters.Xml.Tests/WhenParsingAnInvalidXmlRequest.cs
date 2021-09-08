using FasTnT.Formatter.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FasTnT.Formatters.Xml.Tests
{
    [TestClass]
    public class WhenParsingAnInvalidXmlRequest : XmlParsingTestCase
    {
        public static readonly string ResourceName = "FasTnT.Formatters.Xml.Tests.Resources.Requests.InvalidRequest.xml";

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);

            Assert.ThrowsException<AggregateException>(() => Task.WaitAll(XmlDocumentParser.Instance.ParseAsync(manifest, default)));
        }
    }
}
