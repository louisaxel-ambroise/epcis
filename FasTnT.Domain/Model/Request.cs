using System;
using System.Collections.Generic;

namespace FasTnT.Domain.Model
{
    public class Request
    {
        public int Id { get; set; }
        public StandardBusinessHeader StandardBusinessHeader { get; set; }
        public DateTime CaptureDate { get; set; }
        public DateTime DocumentTime { get; set; }
        public string SchemaVersion { get; set; }
        public SubscriptionCallback SubscriptionCallback { get; set; }
        public List<Event> Events { get; set; } = new();
        public List<MasterData> Masterdata { get; set; } = new();
    }
}
