using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using System.Threading.Tasks;

namespace FasTnT.Application.Tests.Context;

public class TestResultSender : IResultSender
{
    public string Name => "TestFormatter";
    public bool ErrorSent { get; set; }
    public bool ResultSent { get; set; }
    public bool Result { get; set; }

    public TestResultSender() : this(true)
    {
    }

    public TestResultSender(bool result)
    {
        Result = result;
    }

    public Task<bool> SendErrorAsync(Subscription context, EpcisException error, CancellationToken cancellationToken)
    {
        ErrorSent = true;
        return Task.FromResult(Result);
    }

    public Task<bool> SendResultAsync(Subscription context, QueryResponse response, CancellationToken cancellationToken)
    {
        ResultSent = true;
        return Task.FromResult(Result);
    }
}
