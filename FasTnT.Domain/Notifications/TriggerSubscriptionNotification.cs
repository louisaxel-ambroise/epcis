using MediatR;

namespace FasTnT.Domain.Notifications;

public class TriggerSubscriptionNotification : INotification
{
    public string[] Triggers { get; }

    public TriggerSubscriptionNotification(string[] triggers)
    {
        Triggers = triggers;
    }
}
