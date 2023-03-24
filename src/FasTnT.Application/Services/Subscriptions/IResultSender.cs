using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Application.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Subscriptions;

public interface IResultSender
{
    public string Name { get; }

    Task<bool> SendResultAsync(Subscription context, QueryResponse response, CancellationToken cancellationToken);
    Task<bool> SendErrorAsync(Subscription context, EpcisException error, CancellationToken cancellationToken);
}
