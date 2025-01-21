using FasTnT.Application.Services.Subscriptions.Schedulers;

namespace FasTnT.Application.Tests.Subscriptions;

[TestClass]
public class TriggeredSubscriptionSchedulerTests
{
    [TestMethod]
    [DataRow("2021-05-01T10:02:51.000Z")]
    [DataRow("2026-01-30T02:54:10.000Z")]
    [DataRow("1968-10-10T23:20:05.000Z")]
    public void ShouldWork(string input)
    {
        var scheduler = new TriggeredSubscriptionScheduler();
        var date = DateTime.Parse(input);
        var nextExecution = scheduler.ComputeNextExecution(date);

        Assert.AreEqual(date.AddMinutes(2), nextExecution);
    }
}
