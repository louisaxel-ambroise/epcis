﻿using FasTnT.Application.Domain.Enumerations;
using FasTnT.Host.Features.v2_0.Communication.Formatters;
using System.Xml.Linq;

namespace FasTnT.Tests.Features.v2_0.Communication.XML;

[TestClass]
public class WhenFormattingAnErrorResponseWithQueryName
{
    public EpcisException Result = new(ExceptionType.NoSuchNameException, string.Empty) { QueryName = "TestQuery" };
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = XmlResponseFormatter.FormatError(Result);
    }

    [TestMethod]
    public void ItShouldReturnAnXElement()
    {
        Assert.IsNotNull(Formatted);
    }

    [TestMethod]
    public void TheXmlShouldBeCorrectlyFormatted()
    {
        Assert.IsTrue(Formatted.Name == XName.Get("NoSuchNameException", "urn:epcglobal:epcis-query:xsd:1"));
        Assert.AreEqual(1, Formatted.Elements().Count());
        Assert.AreEqual(Result.QueryName, Formatted.Element("queryName").Value);
    }

    [TestMethod]
    public void ThereShouldNotBeASubscriptionIDField()
    {
        Assert.IsNull(Formatted.Element("subscriptionID"));
    }

    [TestMethod]
    public void ThereShouldNotBeAReasonField()
    {
        Assert.IsNull(Formatted.Element("reason"));
    }
}
