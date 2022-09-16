using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.Subscriptions;

public interface IResultSender
{
    public string Name { get; }

    Task<bool> SendResultAsync(ExecutionContext context, QueryResponse response, CancellationToken cancellationToken);
    Task<bool> SendErrorAsync(ExecutionContext context, EpcisException error, CancellationToken cancellationToken);
}
