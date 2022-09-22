using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Subscriptions;

public record SubscriptionContext(int Id, IResultSender ResultSender, SubscriptionSchedule Schedule);