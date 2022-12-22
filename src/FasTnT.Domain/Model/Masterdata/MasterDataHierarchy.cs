namespace FasTnT.Domain.Model.Masterdata;

public class MasterDataHierarchy
{
    public string Root { get; set; }
    public string Type { get; set; }
    public string Id { get; set; }
}

public class BizLocationHierarchy : MasterDataHierarchy { }
public class ReadPointHierarchy : MasterDataHierarchy { }