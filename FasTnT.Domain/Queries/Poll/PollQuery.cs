using MediatR;
using System.Collections.Generic;

namespace FasTnT.Application.Queries.Poll
{
    public record PollQuery(string QueryName, IEnumerable<QueryParameter> Parameters) : IRequest<PollResponse>;

    public record QueryParameter(string Name, string[] Values);
}
