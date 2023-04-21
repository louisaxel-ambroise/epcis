namespace FasTnT.Domain.Model.Masterdata;

public class MasterDataField
{
    public int Index { get; set; }
    public int? ParentIndex { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string Value { get; set; }
}
