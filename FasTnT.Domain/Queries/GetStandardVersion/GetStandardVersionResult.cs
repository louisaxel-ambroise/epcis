using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Queries.GetStandardVersion;

public record GetStandardVersionResult(string Version) : IEpcisResponse;
