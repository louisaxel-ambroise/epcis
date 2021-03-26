using System;

namespace FasTnT.Domain.Exceptions
{
    public class EpcisException : Exception
    {
        public static readonly EpcisException Default = new EpcisException(ExceptionType.ImplementationException, string.Empty, ExceptionSeverity.Error);

        public ExceptionType ExceptionType { get; }
        public ExceptionSeverity Severity { get; }

        public EpcisException(ExceptionType exceptionType, string message, ExceptionSeverity severity = null) : base(message)
        {
            ExceptionType = exceptionType;
            Severity = severity ?? ExceptionSeverity.Error;
        }
    }
}
