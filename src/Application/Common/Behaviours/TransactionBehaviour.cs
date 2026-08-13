using Application.Common.Interfaces;
using Application.Common.Messaging;
using MediatR;

namespace Application.Common.Behaviours;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IAppDbContext _context;

    public TransactionBehaviour(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICommand || _context.HasActiveTransaction)
        {
            return await next();
        }

        using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        // No try/catch: an escaping exception disposes the transaction without
        // committing, which is the rollback we want.
        var response = await next();
        await _context.CommitTransactionAsync(transaction, cancellationToken);

        return response;
    }
}
