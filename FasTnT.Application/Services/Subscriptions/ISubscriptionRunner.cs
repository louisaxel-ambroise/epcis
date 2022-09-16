namespace FasTnT.Application.Services.Subscriptions;

public interface ISubscriptionRunner
{
    Task RunAsync(ExecutionContext executionContext, IResultSender resultSender, CancellationToken cancellationToken);
}
