
using Microsoft.EntityFrameworkCore;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;
using Mundial.Infrastructure.Persistence;

namespace Mundial.Infrastructure.Repositories;

public class PedidoCompraRepository : IPedidoCompraRepository
{
    private readonly MundialDbContext _context;

    public PedidoCompraRepository(MundialDbContext context)
    {
        _context = context;
    }

    public Task<PedidoCompra?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.PedidosCompra
            .Include(p => p.Fornecedor)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                    .ThenInclude(produto => produto.Estoque)
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);
    }

    
public async Task<PedidoCompra?> ObterPorNumeroAsync(
    string numero,
    CancellationToken cancellationToken = default)
{
    return await _context.PedidosCompra
        .Include(p => p.Fornecedor)
        .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
                .ThenInclude(produto => produto.Estoque)
        .FirstOrDefaultAsync(
            p => p.Numero == numero,
            cancellationToken);
}


    public async Task AdicionarAsync(
        PedidoCompra pedido,
        CancellationToken cancellationToken = default)
    {
        await _context.PedidosCompra.AddAsync(
            pedido,
            cancellationToken);
    }
}
