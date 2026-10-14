
using Mundial.Application.Interfaces;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;

namespace Mundial.Application.Services;

public class ProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoService(
        IProdutoRepository produtoRepository,
        IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CriarAsync(
        string codigo,
        string descricao,
        decimal preco,
        CancellationToken cancellationToken = default)
    {
        codigo = codigo.Trim();

        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException(
                "O código do produto é obrigatório.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException(
                "A descrição do produto é obrigatória.");

        if (preco < 0)
            throw new ArgumentException(
                "O preço não pode ser negativo.");

        if (await _produtoRepository.ExistePorCodigoAsync(
            codigo, cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe um produto com este código.");
        }

        var produto = new Produto(codigo, descricao.Trim(), preco);

        await _produtoRepository.AdicionarAsync(
            produto, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return produto.Id;
    }

    public Task<List<Produto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        return _produtoRepository.ObterTodosAsync(cancellationToken);
    }

}
