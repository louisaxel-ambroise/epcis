using FasTnT.Domain.Model;

namespace FasTnT.Application.Commands
{
    public class CaptureEpcisRequestCommand : ICommand<CaptureEpcisRequestResponse>
    {
        public Request Request { get; init; }
    }
}
