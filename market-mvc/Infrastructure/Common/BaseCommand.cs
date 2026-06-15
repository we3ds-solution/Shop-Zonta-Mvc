using MediatR;

namespace market_mvc.Infrastructure.Common
{
    public abstract class BaseCommand : IRequest
    {
    }

    public abstract class BaseCommand<TResponse> : IRequest<TResponse>
    {
    }
}

