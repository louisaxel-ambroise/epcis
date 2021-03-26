using MediatR;

namespace FasTnT.Application
{
    public interface ICommand<TResponse> : IRequest<TResponse> { }
}
