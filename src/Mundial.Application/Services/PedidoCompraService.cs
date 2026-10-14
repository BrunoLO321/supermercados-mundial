using Mundial.Application.DTOs.Pedidos;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Domain.Entities;
using Mundial.Application.Interfaces;

namespace Mundial.Application.Services;

public class PedidoCompraService
{
    private readonly IPedidoCompraRepository _pedidoRepository;
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PedidoCompraService(
        IPedidoCompraRepository pedidoRepository,
        IFornecedorRepository fornecedorRepository,
        IProdutoRepository produtoRepository,
        IUnitOfWork unitOfWork)
    {
        _pedidoRepository = pedidoRepository;
        _fornecedorRepository = fornecedorRepository;
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CriarAsync(
        CriarPedidoRequest request,
        CancellationToken cancellationToken = default)
    {
        var fornecedor =
            await _fornecedorRepository.ObterPorIdAsync(
                request.FornecedorId,
                cancellationToken);

        if (fornecedor is null)
            throw new InvalidOperationException(
                "Fornecedor não encontrado.");

        if (!fornecedor.Ativo)
            throw new InvalidOperationException(
                "Fornecedor está inativo.");

        var pedidoExistente =
            await _pedidoRepository.ObterPorNumeroAsync(
                request.Numero,
                cancellationToken);

        if (pedidoExistente is not null)
            throw new InvalidOperationException(
                "Já existe um pedido com este número.");

        var produtoIds = request.Itens
            .Select(x => x.ProdutoId)
            .Distinct()
            .ToList();

        var produtos =
            await _produtoRepository.ObterPorIdsAsync(
                produtoIds,
                cancellationToken);

        if (produtos.Count != produtoIds.Count)
            throw new InvalidOperationException(
                "Um ou mais produtos não foram encontrados.");

        var pedido = new PedidoCompra(
            request.Numero,
            fornecedor);

        foreach (var item in request.Itens)
        {
            var produto = produtos.First(
                x => x.Id == item.ProdutoId);

            if (!produto.PermiteCompra())
            {
                throw new InvalidOperationException(
                    $"O produto {produto.Codigo} não permite novas compras.");
            }

            pedido.AdicionarItem(
                produto,
                item.Quantidade,
                item.PrecoUnitario);
        }

        await _pedidoRepository.AdicionarAsync(
            pedido,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return pedido.Id;
    }
}