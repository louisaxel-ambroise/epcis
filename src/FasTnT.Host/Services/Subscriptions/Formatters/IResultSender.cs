using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Host.Services.Subscriptions.Formatters;

public interface IResultSender
{
    public string Name { get; }

    Task<bool> SendResultAsync(Subscription context, QueryResponse response, CancellationToken cancellationToken);
    Task<bool> SendErrorAsync(Subscription context, EpcisException error, CancellationToken cancellationToken);
}
