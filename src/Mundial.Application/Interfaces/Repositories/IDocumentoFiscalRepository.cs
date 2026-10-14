
using Mundial.Domain.Entities;

namespace Mundial.Application.Interfaces.Repositories;

public interface IDocumentoFiscalRepository
{
    Task<bool> ExistePorChaveAcessoAsync(
        string chaveAcesso,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        DocumentoFiscal documentoFiscal,
        CancellationToken cancellationToken = default);
}
