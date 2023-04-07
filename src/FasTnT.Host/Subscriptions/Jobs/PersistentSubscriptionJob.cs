using FasTnT.Application;
using FasTnT.Application.Database;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Subscriptions.Formatters;
using FasTnT.Host.Subscriptions.Schedulers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FasTnT.Host.Subscriptions.Jobs;

public class PersistentSubscriptionJob
{
    private readonly Subscription _subscription;
    private readonly ISubscriptionFormatter _formatter;
    private readonly HttpClient _httpClient;
    private readonly HMACSHA256 _hmac;

    public PersistentSubscriptionJob(Subscription subscription)
    {
        _subscription = subscription;
        _httpClient = GetHttpClient(subscription);
        _hmac = !string.IsNullOrEmpty(subscription.SignatureToken)
            ? new HMACSHA256(Encoding.UTF8.GetBytes(_subscription.SignatureToken))
            : null;
        _formatter = subscription.FormatterName == nameof(XmlSubscriptionFormatter)
            ? XmlSubscriptionFormatter.Instance
            : JsonSubscriptionFormatter.Instance;
    }

    public async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var scheduler = SubscriptionScheduler.Create(_subscription);

        EpcisEvents.OnRequestCaptured += scheduler.OnRequestCaptured;
        cancellationToken.Register(scheduler.Stop);

        try
        {
            while (!scheduler.Stopped)
            {
                var executionDate = DateTime.UtcNow;

                if (scheduler.IsDue())
                {
                    var minRecordTime = _subscription.LastExecutedTime.Subtract(TimeSpan.FromSeconds(10));
                    var parameters = _subscription.Parameters.Union(new[]
                    {
                        QueryParameter.Create("GE_recordTime", minRecordTime.ToString()),
                    });

                    try
                    {
                        using var scope = serviceProvider.CreateScope();
                        using var context = scope.ServiceProvider.GetService<EpcisContext>();

                        var runner = scope.ServiceProvider.GetService<SubscriptionRunner>();
                        var result = await runner.ExecuteAsync(new(parameters, _subscription.BufferRequestIds), cancellationToken);

                        if (result.Successful)
                        {
                            if (result.Events.Any() || _subscription.ReportIfEmpty)
                        {
                                await SendResults(result.Events, cancellationToken);
                        }

                            using var context = scope.ServiceProvider.GetService<EpcisContext>();

                        context.Attach(_subscription);

                            _subscription.BufferRequestIds = result.RequestIds.ToArray();
                        _subscription.LastExecutedTime = executionDate;

                        await context.SaveChangesAsync(cancellationToken);
                    }
                        else
                    {
                            await SendError(result.Exception, cancellationToken);
                        }
                    }
                    finally
                    {
                        scheduler.ComputeNextExecution(executionDate);
                    }
                }

                scheduler.WaitForNotification();
            }
        }
        finally
        {
            EpcisEvents.OnRequestCaptured -= scheduler.OnRequestCaptured;
        }
    }

    private static HttpClient GetHttpClient(Subscription subscription)
    {
        var client = new HttpClient { BaseAddress = new Uri(subscription.Destination) };

        if (!string.IsNullOrEmpty(client.BaseAddress.UserInfo))
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(client.BaseAddress.UserInfo)));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
        }

        return client;
    }

    private Task SendResults(List<Event> events, CancellationToken cancellationToken)
    {
        var formatted = _formatter.FormatResult(_subscription.Name, new(_subscription.QueryName, events));

        return SendWebhook(formatted, _formatter.ContentType, cancellationToken);
    }

    private Task SendError(EpcisException ex, CancellationToken cancellationToken)
    {
        var formatted = _formatter.FormatError(_subscription.Name, _subscription.QueryName, ex);

        return SendWebhook(formatted, _formatter.ContentType, cancellationToken);
    }

    private Task SendWebhook(string formatted, string contentType, CancellationToken cancellationToken)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, string.Empty)
        {
            Content = new StringContent(formatted, Encoding.UTF8, contentType)
        };

        if (_hmac is not null)
        {
            var hash = _hmac.ComputeHash(Encoding.UTF8.GetBytes(formatted));

            message.Headers.Add("GS1-Signature", Convert.ToBase64String(hash));
        }

        return _httpClient.SendAsync(message, cancellationToken);
    }
}
