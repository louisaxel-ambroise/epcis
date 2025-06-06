﻿using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Domain.Model;

public class Request
{
    public int Id { get; set; }
    public string CaptureId { get; set; } = Guid.NewGuid().ToString();
    public DateTime RecordTime { get; set; }
    public string UserId { get; set; }
    public StandardBusinessHeader StandardBusinessHeader { get; set; }
    public DateTime DocumentTime { get; set; }
    public string SchemaVersion { get; set; }
    public SubscriptionCallback SubscriptionCallback { get; set; }
    public List<Event> Events { get; set; } = [];
    public List<MasterData> Masterdata { get; set; } = [];
}