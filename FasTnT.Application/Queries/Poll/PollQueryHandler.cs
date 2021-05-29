using FasTnT.Application.Services;
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

        public PollQueryHandler(IEnumerable<IEpcisQuery> queries) => _queries = queries;

        public Task<PollResponse> Handle(PollQuery request, CancellationToken cancellationToken)
        {
            var query = _queries.FirstOrDefault(q => q.Name == request.QueryName)
                           ?? throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{request.QueryName}' is not implemented");

            return query.HandleAsync(request.Parameters, cancellationToken);
        }
    }
}
