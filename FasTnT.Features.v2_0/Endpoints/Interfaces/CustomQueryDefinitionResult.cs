using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record CustomQueryDefinitionResult(string Name, IEnumerable<QueryParameter> Parameters);
