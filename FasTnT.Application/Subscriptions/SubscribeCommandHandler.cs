using FasTnT.Application.Services;
using FasTnT.Domain.Commands.Subscribe;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Notifications;
using FasTnT.Domain.Queries;
using FasTnT.Infrastructure.Store;
using MediatR;

namespace FasTnT.Application.Subscriptions;

public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, IEpcisResponse>
{
    private readonly EpcisContext _context;
    private readonly IEnumerable<IEpcisQuery> _queries;
    private readonly IMediator _mediator;

    public SubscribeCommandHandler(EpcisContext context, IEnumerable<IEpcisQuery> queries, IMediator mediator)
    {
        _context = context;
        _queries = queries;
        _mediator = mediator;
    }

    public async Task<IEpcisResponse> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        EnsureSubscriptionDoesNotExist(request);
        EnsureQueryAllowsSubscription(request);

        var subscription = MapToSubscription(request);
        _context.Subscriptions.Add(subscription);

        await _context.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new SubscriptionCreatedNotification(subscription.Id), cancellationToken);

        return new SubscribeResult();
    }

    private void EnsureSubscriptionDoesNotExist(SubscribeCommand request)
    {
        if(_context.Subscriptions.Any(x => x.Name == request.SubscriptionId))
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, "Subscription already exist");
        }
    }

    private void EnsureQueryAllowsSubscription(SubscribeCommand request)
    {
        var query = _queries.SingleOrDefault(x => x.Name == request.QueryName);

        if(query == default)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, "Query does not exist");
        }
        if(!query.AllowSubscription)
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, "Query does not allow subscription");
        }
    }

    private static Subscription MapToSubscription(SubscribeCommand request)
    {
        return new()
        {
            Name = request.SubscriptionId,
            QueryName = request.QueryName,
            RecordIfEmpty = request.ReportIfEmpty,
            InitialRecordTime = request.InitialRecordTime,
            Schedule = request.Schedule != default ? MapSchedule(request.Schedule) : null,
            Trigger = request.Trigger,
            Destination = request.Destination,
            Parameters = MapParameters(request.Parameters)
        };
    }

    private static List<SubscriptionParameter> MapParameters(List<QueryParameter> parameters)
    {
        return parameters.Select(x => new SubscriptionParameter
        {
            Name = x.Name,
            Value = x.Values
        }).ToList();
    }

    private static SubscriptionSchedule MapSchedule(QuerySchedule schedule)
    {
        return new()
        {
            DayOfMonth = schedule.DayOfMonth,
            DayOfWeek = schedule.DayOfWeek,
            Hour = schedule.Hour,
            Minute = schedule.Minute,
            Month = schedule.Month,
            Second = schedule.Second
        };
    }
}
