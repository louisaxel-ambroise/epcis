namespace FasTnT.Domain.Model.Masterdata;

public class MasterDataField
{
    public MasterDataAttribute Attribute { get; set; }
    public string ParentName { get; set; }
    public string ParentNamespace { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string Value { get; set; }
    public int Index { get; set; }
}
