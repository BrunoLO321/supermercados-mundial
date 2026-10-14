
using Mundial.Domain.Entities;
using Mundial.Domain.Enums;
using Xunit;

namespace Mundial.UnitTests;

public class ProdutoTests
{
    private static Produto CriarProduto()
    {
        return new Produto(
            "TESTE-003",
            "Produto de Teste",
            10m);
    }

    [Fact]
    public void ProdutoNovo_DevePermitirCompraERecebimento()
    {
        var produto = CriarProduto();

        Assert.Equal(StatusProduto.Ativo, produto.Status);
        Assert.True(produto.PermiteCompra());
        Assert.True(produto.PermiteRecebimento());
    }

    [Fact]
    public void ProdutoSuspenso_DeveBloquearCompraEPermitirRecebimento()
    {
        var produto = CriarProduto();

        produto.AtualizarStatus(StatusProduto.SuspensoParaCompra);

        Assert.False(produto.PermiteCompra());
        Assert.True(produto.PermiteRecebimento());
    }

    [Fact]
    public void ProdutoInativo_DeveBloquearCompraERecebimento()
    {
        var produto = CriarProduto();

        produto.AtualizarStatus(StatusProduto.Inativo);

        Assert.False(produto.PermiteCompra());
        Assert.False(produto.PermiteRecebimento());
    }
}
