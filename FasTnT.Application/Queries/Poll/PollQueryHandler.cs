using FasTnT.Application.Services;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries.Poll;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.Poll
{
    public class PollQueryHandler : IRequestHandler<PollQuery, PollResponse>
    {
        private readonly IEnumerable<IEpcisQuery> _queries;
        private readonly ICurrentUser _currentUser;

        public PollQueryHandler(IEnumerable<IEpcisQuery> queries, ICurrentUser currentUser)
        {
            _queries = queries;
            _currentUser = currentUser;
        }

        public Task<PollResponse> Handle(PollQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters.Union(_currentUser.DefaultQueryParameters);
            var query = _queries.FirstOrDefault(q => q.Name == request.QueryName)
                           ?? throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{request.QueryName}' is not implemented");

            return query.HandleAsync(parameters, cancellationToken);
        }
    }
}
