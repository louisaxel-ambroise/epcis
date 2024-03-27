using FasTnT.Domain.Model.Queries;

namespace FasTnT.Host.Endpoints.Interfaces;

public record CustomQueryDefinitionResult(string Name, IEnumerable<QueryParameter> Parameters);
