using FasTnT.Application.Services;
using FasTnT.Domain.Model;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.Poll
{
    public class SimpleMasterDataQuery : IEpcisQuery
    {
        private readonly EpcisContext _context;

        public SimpleMasterDataQuery(EpcisContext context)
        {
            _context = context;
        }

        public string Name => nameof(SimpleMasterDataQuery);
        public bool AllowSubscription => false;

        // TODO: apply parameters
        public async Task<PollResponse> HandleAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
        {
            var query = _context.MasterData.AsNoTracking();

            var result = await query
                .Include(x => x.Attributes)
                .ThenInclude(x => x.Fields)
                .ToListAsync(cancellationToken);

            return new(Name) { VocabularyList = result };
        }
    }
}
