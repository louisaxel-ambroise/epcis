using MediatR;
using Microsoft.Extensions.Logging;

namespace FasTnT.Domain.Infrastructure.Behaviors;

public class CommandLoggerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;

    public CommandLoggerBehavior(ILogger<CommandLoggerBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("Handling {Name}", typeof(TRequest).Name);

        try
        {
            return next();
        }
        finally
        {
            _logger.LogInformation("Handled {Name}", typeof(TRequest).Name);
        }
    }
}
