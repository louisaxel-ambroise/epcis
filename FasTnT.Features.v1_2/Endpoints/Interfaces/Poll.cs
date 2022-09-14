using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces.Queries;

public record Poll(string QueryName, IEnumerable<QueryParameter> Parameters);
