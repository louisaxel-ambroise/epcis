using FasTnT.Application.Services.Storage;

namespace FasTnT.Application.Services.Subscriptions;

public interface ISubscriptionContext
{
    string Name { get; }
    string Trigger { get; }

    bool IsScheduled();
    Task ExecuteAsync(EpcisContext context, DateTime executionTime, CancellationToken cancellationToken);
    DateTime GetNextOccurence(DateTime executionTime);
}
