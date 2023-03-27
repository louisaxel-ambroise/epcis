using FasTnT.Application.Domain.Model;

namespace FasTnT.Application.Services.Subscriptions;

public sealed class EpcisEvents
{
    public static EpcisEvents Instance { get; } = new();

    public event EventHandler<Request> OnRequestCaptured;
    public event EventHandler<ISubscriptionContext> OnSubscriptionRegistered;
    public event EventHandler<string> OnSubscriptionRemoved;
    public event EventHandler<IEnumerable<string>> OnSubscriptionTriggered;

    private EpcisEvents() { }

    public void NotifyCapture(Request request)
    {
        OnRequestCaptured?.Invoke(this, request);
        Trigger(new[] { "stream" });
    }
    public void Register(ISubscriptionContext context) => OnSubscriptionRegistered?.Invoke(this, context);
    public void Remove(string subscriptionName) => OnSubscriptionRemoved?.Invoke(this, subscriptionName);
    public void Trigger(IEnumerable<string> triggers) => OnSubscriptionTriggered?.Invoke(this, triggers);
}
