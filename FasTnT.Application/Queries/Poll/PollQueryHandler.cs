using FasTnT.Application.Services;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;
using MediatR;

namespace FasTnT.Application.Queries;

public class PollQueryHandler : IRequestHandler<PollQuery, IEpcisResponse>
{
    private readonly IEnumerable<Services.IEpcisQuery> _queries;
    private readonly ICurrentUser _currentUser;

    public PollQueryHandler(IEnumerable<Services.IEpcisQuery> queries, ICurrentUser currentUser)
    {
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<IEpcisResponse> Handle(PollQuery request, CancellationToken cancellationToken)
    {
        var parameters = request.Parameters.Union(_currentUser.DefaultQueryParameters);
        var query = _queries.FirstOrDefault(q => q.Name == request.QueryName)
                        ?? throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{request.QueryName}' is not implemented");

        return await query.HandleAsync(parameters, cancellationToken);
    }
}
