using FasTnT.Domain.Model;

namespace FasTnT.Host.Endpoints.Interfaces;

public record ListCapturesResult(IEnumerable<Request> Captures);
