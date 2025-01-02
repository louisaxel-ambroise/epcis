namespace FasTnT.Domain.Model.Masterdata;

public class MasterData
{
    public const string Location = "urn:epcglobal:epcis:vtype:BusinessLocation";
    public const string ReadPoint = "urn:epcglobal:epcis:vtype:ReadPoint";

    public Request Request { get; set; }
    public string Type { get; set; }
    public string Id { get; set; }

    public List<MasterDataAttribute> Attributes { get; set; } = [];
    public List<MasterDataChildren> Children { get; set; } = [];
}
