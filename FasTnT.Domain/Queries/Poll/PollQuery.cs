using MediatR;

namespace FasTnT.Domain.Queries.Poll;

public interface IEpcisQuery { }

public record PollQuery(string QueryName, IEnumerable<QueryParameter> Parameters) : IEpcisQuery, IRequest<IEpcisResponse>;
public record QueryParameter(string Name, string[] Values);
