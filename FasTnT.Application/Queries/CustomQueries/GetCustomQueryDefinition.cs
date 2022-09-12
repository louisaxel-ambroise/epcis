using FasTnT.Application.Services.Queries;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Queries.CustomQueries;
using FasTnT.Domain.Queries.Poll;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Queries.CustomQueries;
public record GetCustomQueryDefinition(string QueryName) : IRequest<IEpcisResponse>;

public class GetCustomQueryDefinitionHandler : IRequestHandler<GetCustomQueryDefinition, IEpcisResponse>
{
    private readonly EpcisContext _context;

    public GetCustomQueryDefinitionHandler(EpcisContext context)
    {
        _context = context;
    }

    public async Task<IEpcisResponse> Handle(GetCustomQueryDefinition request, CancellationToken cancellationToken)
    {
        var query = await _context.CustomQueries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .SingleOrDefaultAsync(x => x.Name == request.QueryName, cancellationToken);

        if(query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query {request.QueryName} does not exist");
        }

        return new CustomQueryDefinitionResult(query);
    }
}

public record PollCustomQuery(string QueryName) : IRequest<IEpcisResponse>;

public class PollCustomQueryHandler : IRequestHandler<PollCustomQuery, IEpcisResponse>
{
    private readonly EpcisContext _context;
    private readonly IEnumerable<Services.IEpcisQuery> _epcisQueries;

    public PollCustomQueryHandler(EpcisContext context, IEnumerable<Services.IEpcisQuery> epcisQueries)
    {
        _context = context;
        _epcisQueries = epcisQueries;
    }

    public async Task<IEpcisResponse> Handle(PollCustomQuery request, CancellationToken cancellationToken)
    {
        var query = await _context.CustomQueries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .SingleOrDefaultAsync(x => x.Name == request.QueryName, cancellationToken);

        if(query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query {request.QueryName} does not exist");
        }

        var eventQuery = _epcisQueries.Single(x => x.Name == nameof(SimpleEventQuery));

        return new PollEventResponse(query.Name, new());
    }
}
