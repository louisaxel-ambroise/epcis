namespace FasTnT.Domain.Model.Subscriptions;

public sealed record SubscriptionSchedule
{
    public string Second { get; init; }
    public string Minute { get; init; }
    public string Hour { get; init; }
    public string DayOfMonth { get; init; }
    public string Month { get; init; }
    public string DayOfWeek { get; init; }
}
