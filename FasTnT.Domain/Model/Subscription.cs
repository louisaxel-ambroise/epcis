using System.Collections.Generic;

namespace FasTnT.Domain.Model
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string QueryName { get; set; }
        public SubscriptionSchedule Schedule { get; set; }
        public string Trigger { get; set; }
        public bool RecordIfEmpty { get; set; }
        public List<SubscriptionParameter> Parameters { get; set; } = new();
    }
}
