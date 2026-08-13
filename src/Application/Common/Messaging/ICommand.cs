using MediatR;

namespace Application.Common.Messaging;

// Marks a request that writes, so TransactionBehaviour can leave queries alone.
public interface ICommand
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>, ICommand
{
}
