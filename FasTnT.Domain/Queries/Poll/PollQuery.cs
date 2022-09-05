using MediatR;

namespace FasTnT.Domain.Queries;

public interface ISoapQuery { }

public record PollQuery(string QueryName, IEnumerable<QueryParameter> Parameters) : ISoapQuery, IRequest<IEpcisResponse>;
public record QueryParameter(string Name, string[] Values);
