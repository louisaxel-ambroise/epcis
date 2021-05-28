using FasTnT.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Infrastructure.Behaviors
{
	public class CommandValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;
		private readonly ILogger<CommandValidationBehavior<TRequest, TResponse>> _logger;

		public CommandValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<CommandValidationBehavior<TRequest, TResponse>> logger)
		{
			_validators = validators;
			_logger = logger;
		}

		public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var context = new ValidationContext<TRequest>(request);
			var failures = _validators
				.Select(v => v.Validate(context))
				.SelectMany(result => result.Errors)
				.Select(error => error.ErrorMessage)
				.Where(f => f != null)
				.ToArray();

			if (failures.Length > 0)
			{
				_logger.LogError($"CommandValidationBehavior: {string.Join(";", failures)}");

				throw new EpcisException(ExceptionType.ValidationException, string.Join(", ", failures));
			}

			return next();
		}
	}
}
