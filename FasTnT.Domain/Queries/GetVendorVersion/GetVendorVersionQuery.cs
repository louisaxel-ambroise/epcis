using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Domain.Queries.GetVendorVersion;

public record GetVendorVersionQuery : IRequest<IEpcisResponse>;
