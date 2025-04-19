namespace FasTnT.Domain.Model.Masterdata;

public class MasterDataAttribute
{
    public int Index { get; set; }
    public string Id { get; set; }
    public string Value { get; set; }
    public List<MasterDataField> Fields { get; set; } = [];
}
