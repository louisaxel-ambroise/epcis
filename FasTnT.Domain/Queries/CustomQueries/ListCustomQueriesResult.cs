using FasTnT.Domain.CutomQueries;
using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Queries.CustomQueries;

public record ListCustomQueriesResult(IEnumerable<CustomQuery> Queries) : IEpcisResponse;
