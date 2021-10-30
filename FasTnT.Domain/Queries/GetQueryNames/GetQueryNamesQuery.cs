using MediatR;

namespace FasTnT.Domain.Queries;

public record GetQueryNamesQuery : IRequest<GetQueryNamesResult>;
