
using Microsoft.EntityFrameworkCore.Storage;
using Mundial.Application.Interfaces;

namespace Mundial.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly MundialDbContext _context;
    private IDbContextTransaction? _transacaoAtual;

    public UnitOfWork(MundialDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IAsyncDisposable> IniciarTransacaoAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transacaoAtual is not null)
        {
            throw new InvalidOperationException(
                "Já existe uma transação em andamento.");
        }

        _transacaoAtual =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        return _transacaoAtual;
    }

    public async Task ConfirmarTransacaoAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transacaoAtual is null)
        {
            throw new InvalidOperationException(
                "Não existe uma transação para confirmar.");
        }

        await _transacaoAtual.CommitAsync(cancellationToken);
        await _transacaoAtual.DisposeAsync();
        _transacaoAtual = null;
    }

    public async Task ReverterTransacaoAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transacaoAtual is null)
        {
            return;
        }

        await _transacaoAtual.RollbackAsync(cancellationToken);
        await _transacaoAtual.DisposeAsync();
        _transacaoAtual = null;
    }
}
