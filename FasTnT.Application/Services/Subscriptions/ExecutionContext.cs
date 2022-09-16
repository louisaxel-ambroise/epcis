using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Subscriptions;

public record ExecutionContext(Subscription Subscription, DateTime DateTime);
