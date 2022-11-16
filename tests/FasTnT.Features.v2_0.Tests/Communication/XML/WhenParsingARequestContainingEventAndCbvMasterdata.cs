using FasTnT.Domain.Model;
using FasTnT.Features.v2_0.Communication.Xml.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FasTnT.Features.v2_0.Tests.Communication.Xml;

[TestClass]
public class WhenParsingARequestContainingEventAndCbvMasterdata : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Features.v2_0.Tests.Resources.Requests.Request_ObjectEvent_WithMasterdata.xml";

    public Request Request { get; set; }

    [TestInitialize]
    public void When()
    {
        Request = XmlEpcisDocumentParser.Parse(ParseResource(ResourceName));
    }

    [TestMethod]
    public void RequestShouldContainsOneEvent()
    {
        Assert.AreEqual(1, Request.Events.Count);
    }

    [TestMethod]
    public void RequestShouldContainsOneMasterdata()
    {
        Assert.AreEqual(1, Request.Masterdata.Count);
    }

    [TestMethod]
    public void ParsedMasterdataShouldMatchTheJsonDocument()
    {
        var masterdata = Request.Masterdata.Single();

        Assert.AreEqual("urn:epcglobal:epcis:vtype:ReadPoint", masterdata.Type);
        Assert.AreEqual("urn:epc:id:sgln:0037000.00729.8001", masterdata.Id);
        Assert.AreEqual(1, masterdata.Attributes.Count);
        Assert.AreEqual("urn:epcglobal:cbv:mda:site", masterdata.Attributes.Single().Id);
        Assert.AreEqual("0037000007296", masterdata.Attributes.Single().Value);
        Assert.AreEqual(0, masterdata.Attributes.Single().Fields.Count);
    }

    [TestMethod]
    public void RequestDateShouldBePopulated()
    {
        var expectedDate = new DateTime(2016, 09, 20, 17, 45, 20, 0);
        Assert.AreEqual(expectedDate, Request.DocumentTime);
    }
}
