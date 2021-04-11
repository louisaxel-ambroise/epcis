using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Exceptions
{
    public class ExceptionSeverity : Enumeration
    {
        public static readonly ExceptionSeverity Error = new(4, "ERROR");
        public static readonly ExceptionSeverity Severe = new(5, "SEVERE");

        public ExceptionSeverity() { }
        public ExceptionSeverity(short id, string displayName) : base(id, displayName) { }
    }
}
