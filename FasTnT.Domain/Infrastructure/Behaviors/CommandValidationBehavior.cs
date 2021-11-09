using FasTnT.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FasTnT.Domain.Infrastructure.Behaviors;

public class CommandValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public CommandValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Select(error => error.ErrorMessage)
            .Where(f => f != null)
            .ToArray();

        if (failures.Length > 0)
        {
            throw new EpcisException(ExceptionType.ValidationException, string.Join(", ", failures));
        }

        return next();
    }
}

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
