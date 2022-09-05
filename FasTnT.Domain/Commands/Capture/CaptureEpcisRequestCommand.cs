using FasTnT.Domain.Model;
using MediatR;

namespace FasTnT.Domain.Commands.Capture;

public class CaptureEpcisRequestCommand : IRequest<CaptureEpcisRequestResponse>
{
    public Request Request { get; init; }
    public CaptureRequestBehavior Behavior { get; set; } = CaptureRequestBehavior.Rollback;
}
