using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Masterdata;
using FasTnT.Application.Domain.Model.Subscriptions;

namespace FasTnT.Application.Domain.Model;

public class Request
{
    public int Id { get; set; }
    public string CaptureId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public StandardBusinessHeader StandardBusinessHeader { get; set; }
    public DateTime CaptureTime { get; set; } = DateTime.UtcNow;
    public DateTime DocumentTime { get; set; }
    public string SchemaVersion { get; set; }
    public SubscriptionCallback SubscriptionCallback { get; set; }
    public List<Event> Events { get; set; } = new();
    public List<MasterData> Masterdata { get; set; } = new();
}
