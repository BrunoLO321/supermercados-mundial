
using Mundial.Application.Interfaces;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;

namespace Mundial.Application.Services;

public class FornecedorService
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FornecedorService(
        IFornecedorRepository fornecedorRepository,
        IUnitOfWork unitOfWork)
    {
        _fornecedorRepository = fornecedorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CriarAsync(
        string cnpj,
        string razaoSocial,
        CancellationToken cancellationToken = default)
    {
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cnpj.Length != 14)
            throw new ArgumentException(
                "O CNPJ deve conter 14 dígitos.");

        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new ArgumentException(
                "A razão social é obrigatória.");

        if (await _fornecedorRepository.ExistePorCnpjAsync(
            cnpj, cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe um fornecedor com este CNPJ.");
        }

        var fornecedor = new Fornecedor(cnpj, razaoSocial.Trim());

        await _fornecedorRepository.AdicionarAsync(
            fornecedor, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return fornecedor.Id;
    }

    public async Task<IReadOnlyList<Fornecedor>> ListarAsync(
        CancellationToken cancellationToken = default)
    {
        return await _fornecedorRepository.ListarAsync(
            cancellationToken);
    }

}
