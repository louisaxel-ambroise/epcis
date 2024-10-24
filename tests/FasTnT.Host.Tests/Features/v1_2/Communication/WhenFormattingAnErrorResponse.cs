﻿using FasTnT.Domain.Exceptions;
using FasTnT.Host.Communication.Xml.Formatters;
using System.Xml.Linq;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenFormattingAnErrorResponse
{
    public EpcisException Result = new(ExceptionType.NoSuchNameException, "Query does not exist");
    public XElement Formatted { get; set; }

    [TestInitialize]
    public void When()
    {
        Formatted = SoapResponseFormatter.FormatError(Result);
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
        Assert.AreEqual(Result.Message, Formatted.Element("reason").Value);
    }

    [TestMethod]
    public void ThereShouldNotBeASubscriptionIDField()
    {
        Assert.IsNull(Formatted.Element("subscriptionID"));
    }

    [TestMethod]
    public void ThereShouldNotBeAQueryNameField()
    {
        Assert.IsNull(Formatted.Element("queryName"));
    }
}
