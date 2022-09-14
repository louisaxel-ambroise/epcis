using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.GetCustomQueryDetails
{
    internal class GetCustomQueryDetailsHandler : IGetCustomQueryDetailsHandler
    {
        private readonly EpcisContext _context;

        public GetCustomQueryDetailsHandler(EpcisContext context)
        {
            _context = context;
        }

        public async Task<CustomQuery> GetCustomQueryDetailsAsync(string name, CancellationToken cancellationToken)
        {
            var customQuery = await _context.CustomQueries
                .AsNoTracking()
                .Include(x => x.Parameters)
                .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

            if(customQuery is null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Query not found: '{name}'");
            }

            return customQuery;
        }
    }
}
