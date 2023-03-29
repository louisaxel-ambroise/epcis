using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Services.Subscriptions;

namespace FasTnT.Tests.Application.Context;

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
