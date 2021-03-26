using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Exceptions
{
    public class ExceptionType : Enumeration
    {
        public static readonly ExceptionType SubscribeNotPermittedException = new ExceptionType(0, nameof(SubscribeNotPermittedException));
        public static readonly ExceptionType ImplementationException = new ExceptionType(1, nameof(ImplementationException));
        public static readonly ExceptionType NoSuchNameException = new ExceptionType(2, nameof(NoSuchNameException));
        public static readonly ExceptionType QueryTooLargeException = new ExceptionType(3, nameof(QueryTooLargeException));
        public static readonly ExceptionType QueryParameterException = new ExceptionType(4, nameof(QueryParameterException));
        public static readonly ExceptionType ValidationException = new ExceptionType(5, nameof(ValidationException));
        public static readonly ExceptionType SubscriptionControlsException = new ExceptionType(6, nameof(SubscriptionControlsException));
        public static readonly ExceptionType NoSuchSubscriptionException = new ExceptionType(7, nameof(NoSuchSubscriptionException));
        public static readonly ExceptionType DuplicateSubscriptionException = new ExceptionType(8, nameof(DuplicateSubscriptionException));
        public static readonly ExceptionType QueryTooComplexException = new ExceptionType(9, nameof(QueryTooComplexException));
        public static readonly ExceptionType InvalidURIException = new ExceptionType(10, nameof(InvalidURIException));

        public ExceptionType() { }
        public ExceptionType(short id, string displayName) : base(id, displayName) { }
    }
}
