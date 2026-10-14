
using Mundial.Domain.Entities;

namespace Mundial.Application.Interfaces.Repositories;

public interface IAcordoComercialRepository
{
    Task AdicionarAsync(
        AcordoComercial acordo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AcordoComercial>> ObterTodosAsync(
        CancellationToken cancellationToken = default);
}
