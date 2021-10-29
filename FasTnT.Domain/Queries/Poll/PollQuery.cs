using MediatR;

namespace FasTnT.Domain.Queries;

public record PollQuery(string QueryName, IEnumerable<QueryParameter> Parameters) : IRequest<PollResponse>;
public record QueryParameter(string Name, string[] Values);
