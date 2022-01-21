namespace FasTnT.Domain.Exceptions;

public class EpcisException : Exception
{
    public static readonly EpcisException Default = new (ExceptionType.ImplementationException, string.Empty) { Severity = ExceptionSeverity.Error };

    public ExceptionType ExceptionType { get; }
    public ExceptionSeverity Severity { get; set; }
    public string QueryName { get; set; }
    public string SubscriptionId { get; set; }

    public EpcisException(ExceptionType exceptionType, string message) : base(message)
    {
        ExceptionType = exceptionType;
    }
}
