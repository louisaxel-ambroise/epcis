using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Subscriptions;


public class TestResultSender : IResultSender
{
    public string Name => "TestFormatter";

    public Task<bool> SendErrorAsync(Subscription context, EpcisException error, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendResultAsync(Subscription context, QueryResponse response, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
