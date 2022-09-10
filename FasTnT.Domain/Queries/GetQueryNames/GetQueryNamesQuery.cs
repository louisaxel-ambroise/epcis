using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Domain.Queries.GetQueryNames;

public record GetQueryNamesQuery : IRequest<IEpcisResponse>;
