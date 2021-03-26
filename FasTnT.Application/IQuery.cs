using MediatR;

namespace FasTnT.Application
{
    public interface IQuery<TResponse> : IRequest<TResponse> { }
}
