
using Microsoft.EntityFrameworkCore;
using Mundial.Application.Interfaces;
using Mundial.Domain.Entities;
using Mundial.Infrastructure.Persistence;
using Mundial.Application.Interfaces.Repositories;

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
