using MediatR;

namespace FasTnT.Domain.Queries;

public interface IEpcisQuery { }

public record PollQuery(string QueryName, IEnumerable<QueryParameter> Parameters) : IEpcisQuery, IRequest<IEpcisResponse>;
public record QueryParameter(string Name, string[] Values);
