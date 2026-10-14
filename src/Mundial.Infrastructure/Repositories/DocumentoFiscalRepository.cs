
using Microsoft.EntityFrameworkCore;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;
using Mundial.Infrastructure.Persistence;

namespace Mundial.Infrastructure.Repositories;

public class DocumentoFiscalRepository : IDocumentoFiscalRepository
{
    private readonly MundialDbContext _context;

    public DocumentoFiscalRepository(MundialDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistePorChaveAcessoAsync(
        string chaveAcesso,
        CancellationToken cancellationToken = default)
    {
        return _context.DocumentosFiscais
            .AnyAsync(
                x => x.ChaveAcesso == chaveAcesso,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        DocumentoFiscal documentoFiscal,
        CancellationToken cancellationToken = default)
    {
        await _context.DocumentosFiscais.AddAsync(
            documentoFiscal,
            cancellationToken);
    }
}
