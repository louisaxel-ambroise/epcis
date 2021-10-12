using System;

namespace FasTnT.Domain.Model
{
    public class SubscriptionExecutionRecord
    {
        public Subscription Subscription { get; set; }
        public DateTime ExecutionTime { get; set; }
        public bool Successful { get; set; }
        public bool ResultsSent { get; set; }
        public string Reason { get; set; }
    }
}
