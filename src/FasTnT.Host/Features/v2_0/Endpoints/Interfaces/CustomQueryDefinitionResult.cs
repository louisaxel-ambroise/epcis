using FasTnT.Domain.Model.Queries;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record CustomQueryDefinitionResult(string Name, IEnumerable<QueryParameter> Parameters);
