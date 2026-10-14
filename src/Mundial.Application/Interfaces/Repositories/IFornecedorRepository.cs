
using Mundial.Domain.Entities;
using Mundial.Application.Interfaces.Repositories;

namespace Mundial.Application.Interfaces.Repositories;

public interface IFornecedorRepository
{
    Task<Fornecedor?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistePorCnpjAsync(
        string cnpj,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Fornecedor fornecedor,
        CancellationToken cancellationToken = default);
}
