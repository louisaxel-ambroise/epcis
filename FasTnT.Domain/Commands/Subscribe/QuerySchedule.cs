using System.Text.RegularExpressions;

namespace FasTnT.Domain.Commands.Subscribe
{
    public class QuerySchedule
    {
        public string Second { get; set; }
        public string Minute { get; set; }
        public string Hour { get; set; }
        public string DayOfMonth { get; set; }
        public string Month { get; set; }
        public string DayOfWeek { get; set; }
    }
}
