using FasTnT.Application.Services.Subscriptions.Schedulers;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Tests.Subscriptions;

[TestClass]
public class CronSubscriptionSchedulerTests
{
    public static SubscriptionSchedule
        Second = new() { Second = "5" },
        Minute = new() { Minute = "24" },
        Hour = new() { Hour = "15" },
        Month = new() { Month = "8" },
        DayOfWeek = new() { DayOfWeek = "4" },
        DayOwMonth = new() { DayOfMonth = "30" };

    [TestMethod]
    public void ShouldWork()
    {
        var scheduler = new CronSubscriptionScheduler(DayOfWeek);
        var date = new DateTime(2025, 01, 10, 00, 09, 00);

        var nextExecution = scheduler.ComputeNextExecution(date);

        Assert.AreEqual(new DateTime(2025, 01, 15, 00, 00, 00), nextExecution);
    }

    [TestMethod]
    public void ShouldStartFromNextSecond()
    {
        var scheduler = new CronSubscriptionScheduler(Second);
        var date = new DateTime(2024, 10, 01, 01, 10, 05);

        var nextExecution = scheduler.ComputeNextExecution(date);

        Assert.AreEqual(new DateTime(2024, 10, 01, 01, 11, 05), nextExecution);
    }

    [TestMethod]
    [DataRow("2021-05-01T10:02:51.000Z")]
    [DataRow("2026-01-30T02:54:10.000Z")]
    [DataRow("1968-10-10T23:20:05.000Z")]
    public void SecondScheduleShouldAlwaysBeMatched(string input)
    {
        var scheduler = new CronSubscriptionScheduler(Second);
        var nextExecution = scheduler.ComputeNextExecution(DateTime.Parse(input));

        Assert.AreEqual(5, nextExecution.Second);
    }

    [TestMethod]
    [DataRow("2021-05-01T10:02:51.000Z")]
    [DataRow("2026-01-30T02:54:10.000Z")]
    [DataRow("1968-10-10T23:20:05.000Z")]
    public void MinuteScheduleShouldAlwaysBeMatched(string input)
    {
        var scheduler = new CronSubscriptionScheduler(Minute);
        var nextExecution = scheduler.ComputeNextExecution(DateTime.Parse(input));

        Assert.AreEqual(24, nextExecution.Minute);
    }

    [TestMethod]
    [DataRow("2021-05-01T10:02:51.000Z")]
    [DataRow("2026-01-30T02:54:10.000Z")]
    [DataRow("1968-10-10T23:20:05.000Z")]
    public void MonthScheduleShouldAlwaysBeMatched(string input)
    {
        var scheduler = new CronSubscriptionScheduler(Month);
        var nextExecution = scheduler.ComputeNextExecution(DateTime.Parse(input));

        Assert.AreEqual(8, nextExecution.Month);
    }
}
