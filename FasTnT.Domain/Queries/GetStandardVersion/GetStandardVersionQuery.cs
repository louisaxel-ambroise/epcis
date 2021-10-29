using MediatR;

namespace FasTnT.Domain.Queries;

public record GetStandardVersionQuery : IRequest<GetStandardVersionResult>;
