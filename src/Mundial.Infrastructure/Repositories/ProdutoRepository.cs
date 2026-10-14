
using Microsoft.EntityFrameworkCore;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;
using Mundial.Infrastructure.Persistence;

namespace Mundial.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly MundialDbContext _context;

    public ProdutoRepository(MundialDbContext context)
    {
        _context = context;
    }

    public Task<Produto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Produtos
            .Include(p => p.Estoque)
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);
    }

    public Task<List<Produto>> ObterPorIdsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        var idsLista = ids.Distinct().ToList();

        return _context.Produtos
            .Include(p => p.Estoque)
            .Where(p => idsLista.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }


    public Task<List<Produto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Produtos
            .Include(p => p.Estoque)
            .AsNoTracking()
            .OrderBy(p => p.Descricao)
            .ToListAsync(cancellationToken);
    }


    public Task<bool> ExistePorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        return _context.Produtos
            .AnyAsync(
                p => p.Codigo == codigo,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        Produto produto,
        CancellationToken cancellationToken = default)
    {
        await _context.Produtos.AddAsync(
            produto,
            cancellationToken);
    }
}
