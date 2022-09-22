namespace FasTnT.Domain.Model.Events;

public class MasterDataField
{
    public MasterDataAttribute Attribute { get; set; }
    public string ParentName { get; set; }
    public string ParentNamespace { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string Value { get; set; }
}
