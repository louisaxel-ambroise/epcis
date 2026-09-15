namespace FasTnT.Domain.Exceptions;

public class EpcisException(ExceptionType exceptionType, string message) : Exception(message)
{
    public static readonly EpcisException Default = new(ExceptionType.ImplementationException, string.Empty) { Severity = ExceptionSeverity.Error };

    public ExceptionType ExceptionType { get; } = exceptionType;
    public ExceptionSeverity Severity { get; set; }
    public string QueryName { get; set; }
    public string SubscriptionId { get; set; }
}
