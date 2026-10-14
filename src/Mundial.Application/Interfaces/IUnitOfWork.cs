
namespace Mundial.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<IAsyncDisposable> IniciarTransacaoAsync(
        CancellationToken cancellationToken = default);

    Task ConfirmarTransacaoAsync(
        CancellationToken cancellationToken = default);

    Task ReverterTransacaoAsync(
        CancellationToken cancellationToken = default);
}
