
using Mundial.Domain.Entities;

namespace Mundial.Application.Interfaces.Repositories;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<Produto>> ObterPorIdsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default);

    Task<List<Produto>> ObterTodosAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistePorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Produto produto,
        CancellationToken cancellationToken = default);
}
