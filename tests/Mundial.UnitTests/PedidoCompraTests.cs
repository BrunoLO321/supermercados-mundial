
using Mundial.Domain.Entities;
using Mundial.Domain.Enums;
using Xunit;

namespace Mundial.UnitTests;

public class PedidoCompraTests
{
    private static (PedidoCompra Pedido, Produto Produto) CriarPedido()
    {
        var fornecedor = new Fornecedor(
            "58464982000152",
            "Fornecedor Teste");

        var produto = new Produto(
            "TESTE-002",
            "Produto de Teste",
            10m);

        var pedido = new PedidoCompra(
            "PED-UNIT-001",
            fornecedor);

        pedido.AdicionarItem(produto, 10m, 8.50m);

        return (pedido, produto);
    }

    [Fact]
    public void CriarPedido_DeveIniciarAberto()
    {
        var (pedido, _) = CriarPedido();

        Assert.Equal(StatusPedido.Aberto, pedido.Status);
    }

    [Fact]
    public void ReceberParteDoPedido_DeveFicarParcial()
    {
        var (pedido, _) = CriarPedido();

        pedido.Itens.First().RegistrarRecebimento(4m);
        pedido.AtualizarStatus();

        Assert.Equal(StatusPedido.Parcial, pedido.Status);
    }

    [Fact]
    public void ReceberTodoOPedido_DeveFicarFechado()
    {
        var (pedido, _) = CriarPedido();

        pedido.Itens.First().RegistrarRecebimento(10m);
        pedido.AtualizarStatus();

        Assert.Equal(StatusPedido.Fechado, pedido.Status);
    }
}
