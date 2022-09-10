using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Queries.GetVendorVersion;

public record GetVendorVersionResult(string Version) : IEpcisResponse;
