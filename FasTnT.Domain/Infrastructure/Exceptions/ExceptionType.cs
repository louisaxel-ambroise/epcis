namespace FasTnT.Domain.Infrastructure.Exceptions;

public enum ExceptionType
{
    SubscribeNotPermittedException,
    ImplementationException,
    NoSuchNameException,
    QueryTooLargeException,
    QueryParameterException,
    ValidationException,
    SubscriptionControlsException,
    NoSuchSubscriptionException,
    DuplicateSubscriptionException,
    QueryTooComplexException,
    InvalidURIException
}
