
using Microsoft.AspNetCore.Mvc;
using Mundial.Application.Services;
using Mundial.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Mundial.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/pedidos-compra")]
public class PedidosCompraController : ControllerBase
{
    private readonly PedidoCompraService _pedidoCompraService;
    private readonly IPedidoCompraRepository _pedidoRepository;

    public PedidosCompraController(
        PedidoCompraService pedidoCompraService,
        IPedidoCompraRepository pedidoRepository)
    {
        _pedidoCompraService = pedidoCompraService;
        _pedidoRepository = pedidoRepository;
    }

    public record CriarItemPedidoRequest(
        int ProdutoId,
        decimal Quantidade,
        decimal PrecoUnitario);

    public record CriarPedidoRequest(
        string Numero,
        int FornecedorId,
        List<CriarItemPedidoRequest> Itens);

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarPedidoRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _pedidoCompraService.CriarAsync(
            new Mundial.Application.DTOs.Pedidos.CriarPedidoRequest(
                request.Numero,
                request.FornecedorId,
                request.Itens
                    .Select(item =>
                        new Mundial.Application.DTOs.Pedidos.CriarItemPedidoRequest(
                            item.ProdutoId,
                            item.Quantidade,
                            item.PrecoUnitario))
                    .ToList()),
            cancellationToken);

        return Created(
            $"/api/pedidos-compra/{id}",
            new
            {
                id,
                mensagem = "Pedido de compra criado com sucesso."
            });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(
            id,
            cancellationToken);

        if (pedido is null)
        {
            return NotFound(new
            {
                mensagem = "Pedido de compra não encontrado."
            });
        }

        return Ok(new
        {
            pedido.Id,
            pedido.Numero,
            Status = pedido.Status.ToString(),
            Fornecedor = pedido.Fornecedor.RazaoSocial,
            Itens = pedido.Itens.Select(item => new
            {
                ProdutoId = item.ProdutoId,
                CodigoProduto = item.Produto.Codigo,
                DescricaoProduto = item.Produto.Descricao,
                item.QuantidadeSolicitada,
                item.QuantidadeRecebida,
                item.QuantidadePendente,
                EstoqueAtual = item.Produto.Estoque?.Quantidade ?? 0,
                item.PrecoUnitario
            })
        });
    }
}
