using FasTnT.Domain.Model.Masterdata;
using FasTnT.Host.Communication.Xml.Parsers;

namespace FasTnT.Host.Tests.Features.v1_2.Communication;

[TestClass]
public class WhenParsingValidMasterDataElement : XmlParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v1_2.Communication.Resources.MasterData.ValidMasterdataElement.xml";

    public IEnumerable<MasterData> MasterData { get; set; }

    [TestInitialize]
    public void When()
    {
        MasterData = XmlMasterdataParser.ParseMasterdata(ParseXml(ResourceName));
    }

    [TestMethod]
    public void ItShouldparseAllMasterdata()
    {
        Assert.AreEqual(4, MasterData.Count());
    }

    [TestMethod]
    public void TheMasterdataTypeSHouldBeParsedCorrectly()
    {
        Assert.AreEqual(1, MasterData.Count(x => x.Type == "urn:epcglobal:epcis:vtype:EPCClass"));
        Assert.AreEqual(3, MasterData.Count(x => x.Type == "urn:epcglobal:epcis:vtype:BusinessLocation"));
    }

    [TestMethod]
    public void EachMasterdataShouldHasAUniqueIndexValue()
    {
        Assert.AreEqual(4, MasterData.DistinctBy(x => x.Index).Count());
    }

    [TestMethod]
    public void ItShouldAllowHavingMultipleTimesTheSameMasterdata()
    {
        var duplicates = MasterData.Where(x => x.Type == "urn:epcglobal:epcis:vtype:BusinessLocation" && x.Id == "urn:epc:id:sgln:0123456.01234.0");
        
        Assert.AreEqual(2, duplicates.Count());
        Assert.IsTrue(duplicates.Any(x => x.Attributes.Single(a => a.Id == "urn:epcglobal:cbv:mda#name").Value == "LOC 2 NAME"));
        Assert.IsTrue(duplicates.Any(x => x.Attributes.Single(a => a.Id == "urn:epcglobal:cbv:mda#name").Value == "LOC 2 NAME BIS"));
    }

    [TestMethod]
    public void ItShouldParseTheFieldsCorrectly()
    {
        var epcClass = MasterData.Single(x => x.Type == "urn:epcglobal:epcis:vtype:EPCClass");
        var attribute = epcClass.Attributes.Single(a => a.Id == "urn:epcglobal:cbv:mda#fields");

        Assert.AreEqual(2, attribute.Fields.Count);
    }
}