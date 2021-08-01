using FasTnT.Application.Services;
using FasTnT.Domain.Queries.GetQueryNames;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.GetQueryNames
{
    public class GetQueryNamesQueryHandler : IRequestHandler<GetQueryNamesQuery, GetQueryNamesResult>
    {
        private readonly IEnumerable<IEpcisQuery> _queries;

        public GetQueryNamesQueryHandler(IEnumerable<IEpcisQuery> queries)
        {
            _queries = queries;
        }

        public Task<GetQueryNamesResult> Handle(GetQueryNamesQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<GetQueryNamesResult>(new(_queries.Select(x => x.Name)));
        }
    }
}
