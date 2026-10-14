
using Microsoft.EntityFrameworkCore;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;
using Mundial.Infrastructure.Persistence;

namespace Mundial.Infrastructure.Repositories;

public class AcordoComercialRepository : IAcordoComercialRepository
{
    private readonly MundialDbContext _context;

    public AcordoComercialRepository(MundialDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(
        AcordoComercial acordo,
        CancellationToken cancellationToken = default)
    {
        await _context.AcordosComerciais.AddAsync(
            acordo,
            cancellationToken);
    }

    public async Task<IReadOnlyList<AcordoComercial>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.AcordosComerciais
            .AsNoTracking()
            .OrderByDescending(a => a.DataCriacao)
            .ToListAsync(cancellationToken);
    }
}
