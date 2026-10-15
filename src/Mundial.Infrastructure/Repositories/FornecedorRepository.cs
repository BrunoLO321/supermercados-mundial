
using Microsoft.EntityFrameworkCore;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;
using Mundial.Infrastructure.Persistence;

namespace Mundial.Infrastructure.Repositories;

public class FornecedorRepository : IFornecedorRepository
{
    private readonly MundialDbContext _context;

    public FornecedorRepository(MundialDbContext context)
    {
        _context = context;
    }

    public Task<Fornecedor?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Fornecedores
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Fornecedor>> ListarAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .AsNoTracking()
            .OrderBy(x => x.RazaoSocial)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistePorCnpjAsync(
        string cnpj,
        CancellationToken cancellationToken = default)
    {
        return _context.Fornecedores
            .AnyAsync(
                x => x.Cnpj == cnpj,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        Fornecedor fornecedor,
        CancellationToken cancellationToken = default)
    {
        await _context.Fornecedores.AddAsync(
            fornecedor,
            cancellationToken);
    }
}
