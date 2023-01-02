namespace FasTnT.Domain.Model.Masterdata;

public class MasterData
{
    public Request Request { get; set; }
    public string Type { get; set; }
    public string Id { get; set; }

    public List<MasterDataAttribute> Attributes { get; set; } = new();
    public List<MasterDataChildren> Children { get; set; } = new();
}

public class BizLocation : MasterData { }
public class ReadPoint : MasterData { }