using FasTnT.Domain.Model.Queries;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public record Poll(string QueryName, IEnumerable<QueryParameter> Parameters);
