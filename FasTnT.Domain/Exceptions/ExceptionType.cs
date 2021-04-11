using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Exceptions
{
    public class ExceptionType : Enumeration
    {
        public static readonly ExceptionType SubscribeNotPermittedException = new(0, nameof(SubscribeNotPermittedException));
        public static readonly ExceptionType ImplementationException = new(1, nameof(ImplementationException));
        public static readonly ExceptionType NoSuchNameException = new(2, nameof(NoSuchNameException));
        public static readonly ExceptionType QueryTooLargeException = new(3, nameof(QueryTooLargeException));
        public static readonly ExceptionType QueryParameterException = new(4, nameof(QueryParameterException));
        public static readonly ExceptionType ValidationException = new(5, nameof(ValidationException));
        public static readonly ExceptionType SubscriptionControlsException = new(6, nameof(SubscriptionControlsException));
        public static readonly ExceptionType NoSuchSubscriptionException = new(7, nameof(NoSuchSubscriptionException));
        public static readonly ExceptionType DuplicateSubscriptionException = new(8, nameof(DuplicateSubscriptionException));
        public static readonly ExceptionType QueryTooComplexException = new(9, nameof(QueryTooComplexException));
        public static readonly ExceptionType InvalidURIException = new(10, nameof(InvalidURIException));

        public ExceptionType() { }
        public ExceptionType(short id, string displayName) : base(id, displayName) { }
    }
}
