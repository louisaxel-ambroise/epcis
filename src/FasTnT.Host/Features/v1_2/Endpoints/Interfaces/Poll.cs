using FasTnT.Domain.Model.Queries;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

public record Poll(string QueryName, IEnumerable<QueryParameter> Parameters);
