using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Domain.Queries.GetStandardVersion;

public record GetStandardVersionQuery : IRequest<IEpcisResponse>;
