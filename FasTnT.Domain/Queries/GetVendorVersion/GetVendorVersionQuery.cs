using MediatR;

namespace FasTnT.Domain.Queries;

public record GetVendorVersionQuery : IRequest<GetVendorVersionResult>;
