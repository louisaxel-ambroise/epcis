using FasTnT.Application.Services.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Context;

public class TestSubscriptionListener : ISubscriptionListener
{
    public List<(string Action, string Value)> _actions = new();

    public bool IsTriggered(string value) => _actions.Any(x => x.Value == value && x.Action == nameof(TriggerAsync));
    public bool IsRemoved(string value) => _actions.Any(x => x.Value == value && x.Action == nameof(RemoveAsync));

    public Task RemoveAsync(string name, CancellationToken _)
    {
        _actions.Add((nameof(RemoveAsync), name));

        return Task.CompletedTask;
    }

    public Task TriggerAsync(string[] triggers, CancellationToken _)
    {
        Array.ForEach(triggers, x => _actions.Add((nameof(TriggerAsync), x)));

        return Task.CompletedTask;
    }
}