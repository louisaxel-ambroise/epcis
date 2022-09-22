namespace FasTnT.Domain.Model.Events;

public class MasterDataAttribute
{
    public MasterData MasterData { get; set; }
    public string Id { get; set; }
    public string Value { get; set; }
    public List<MasterDataField> Fields { get; set; } = new();
}
