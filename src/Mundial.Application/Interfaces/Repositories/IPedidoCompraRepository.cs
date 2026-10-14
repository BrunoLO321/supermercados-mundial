
using Mundial.Domain.Entities;

namespace Mundial.Application.Interfaces.Repositories;

public interface IPedidoCompraRepository
{
    Task<PedidoCompra?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PedidoCompra?> ObterPorNumeroAsync(
        string numero,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        PedidoCompra pedido,
        CancellationToken cancellationToken = default);
}
