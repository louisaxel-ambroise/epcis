using FasTnT.Application.Database;
using FasTnT.Application.Services.Notifications;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Services.Subscriptions.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FasTnT.Host.Services.Subscriptions;

public class SubscriptionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionBackgroundService> _logger;
    private readonly List<Subscription> _subscriptions = new();
    private readonly Dictionary<int, ISubscriptionScheduler> _schedulers = new();

    public SubscriptionBackgroundService(IServiceProvider serviceProvider, ILogger<SubscriptionBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LoadSubscriptions();

        var listener = _serviceProvider.GetService<INotificationReceiver>();
        listener.OnRequestCaptured += TriggerExecution;
        listener.OnSubscriptionRegistered += RegisterSubscription;
        listener.OnSubscriptionRemoved += RemoveSubscription;

        stoppingToken.Register(() => { lock (_subscriptions) { Monitor.Pulse(_subscriptions); } });

        return Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Run(stoppingToken).ContinueWith(_ => WaitForNextExecution());
            }
        }, stoppingToken);
    }

    private void WaitForNextExecution()
    {
        lock (_subscriptions)
        {
            var nextExecution = _subscriptions.Any()
                ? _subscriptions.Min(x => x.NextExecutionTime)
                : DateTime.UtcNow.AddSeconds(30);

            var delay = nextExecution - DateTime.UtcNow >= TimeSpan.FromHours(1)
                ? TimeSpan.FromHours(1)
                : nextExecution - DateTime.UtcNow;

            if (delay > TimeSpan.Zero)
            {
                _logger.LogInformation("Subscription task waiting for {delay} or until next trigger", delay);
                Monitor.Wait(_subscriptions, delay);
            }
        }
    }

    private void LoadSubscriptions()
    {
        using var scope = _serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        context.Set<Subscription>().ToList().ForEach(RegisterSubscription);
    }

    private void TriggerExecution(Request _)
    {
        lock (_subscriptions)
        {
            Monitor.Pulse(_subscriptions);
        }
    }

    private void RegisterSubscription(Subscription subscription)
    {
        lock (_subscriptions)
        {
            _subscriptions.Add(subscription);
            _schedulers.Add(subscription.Id, ISubscriptionScheduler.Get(subscription.Schedule));

            Monitor.Pulse(_subscriptions);
        }
    }

    private void RemoveSubscription(Subscription subscription)
    {
        lock (_subscriptions) 
        {
            var entry = _subscriptions.Single(x => x.Name == subscription.Name);
            _subscriptions.Remove(entry);
            _schedulers.Remove(entry.Id);

            Monitor.Pulse(_subscriptions);
        }
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var executionTime = DateTime.UtcNow;
        var triggeredSubscriptions = default(IEnumerable<Subscription>);

        lock (_subscriptions)
        {
            // TODO: check for stream subscriptions
            triggeredSubscriptions = _subscriptions.Where(x => x.NextExecutionTime <= executionTime).ToList();
        }

        try
        {
            var tasks = triggeredSubscriptions.Select(subscription => Task.Run(async () =>
            {
                var minRecordTime = subscription.LastExecutedTime.Subtract(TimeSpan.FromSeconds(10));

                var parameters = subscription.Parameters.Union(new[]
                {
                    QueryParameter.Create("GE_recordTime", minRecordTime.ToString()),
                });

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    using var context = scope.ServiceProvider.GetService<EpcisContext>();

                    var pendingEvents = await context.QueryEvents(parameters)
                        .Select(x => new { x.Id, RequestId = x.Request.Id })
                        .ToListAsync(cancellationToken);
                    var eventIds = pendingEvents
                        .Where(x => !subscription.BufferRequestIds.Contains(x.RequestId))
                        .Select(x => x.Id);
                    var events = await context.Set<Event>()
                        .Where(x => eventIds.Contains(x.Id))
                        .ToListAsync(cancellationToken);

                    if (events.Any() || subscription.ReportIfEmpty)
                    {
                        await SendResults(subscription, events, cancellationToken);
                    }

                    context.Attach(subscription);

                    subscription.NextExecutionTime = _schedulers[subscription.Id].GetNextExecution(executionTime);
                    subscription.BufferRequestIds = pendingEvents.Select(e => e.RequestId).Distinct().ToArray();
                    subscription.LastExecutedTime = executionTime;

                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (EpcisException ex)
                {
                    await SendError(subscription, ex, cancellationToken);
                }
            }, cancellationToken));

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            // Don't throw the exception as we want the background process to continue
            _logger.LogError(ex, "An error occured while executing subscriptions");
        }
    }

    private Task SendResults(Subscription subscription, List<Event> events, CancellationToken cancellationToken)
    {
        var formatter = GetFormatter(subscription.FormatterName);
        var formatted = formatter.FormatResult(subscription.Name, new(subscription.QueryName, events));

        return SendWebhook(subscription, formatted, formatter.ContentType, cancellationToken);
    }

    private Task SendError(Subscription subscription, EpcisException ex, CancellationToken cancellationToken)
    {
        var formatter = GetFormatter(subscription.FormatterName);
        var formatted = formatter.FormatError(subscription.Name, subscription.QueryName, ex);

        return SendWebhook(subscription, formatted, formatter.ContentType, cancellationToken);
    }

    private static Task SendWebhook(Subscription subscription, string formatted, string contentType, CancellationToken cancellationToken)
    {
        var client = new HttpClient { BaseAddress = new Uri(subscription.Destination) };

        if (!string.IsNullOrEmpty(client.BaseAddress.UserInfo))
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(client.BaseAddress.UserInfo)));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
        }

        var message = new HttpRequestMessage(HttpMethod.Post, string.Empty);
        message.Content = new StringContent(formatted, Encoding.UTF8, contentType);

        if (!string.IsNullOrEmpty(subscription.SignatureToken))
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(subscription.SignatureToken));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(formatted));

            message.Headers.Add("GS1-Signature", Convert.ToBase64String(hash));
        }

        
        return client.SendAsync(message, cancellationToken);
    }

    private ISubscriptionFormatter GetFormatter(string formatterName)
    {
        return formatterName == nameof(XmlSubscriptionFormatter)
            ? XmlSubscriptionFormatter.Instance
            : JsonSubscriptionFormatter.Instance;
    }
}
