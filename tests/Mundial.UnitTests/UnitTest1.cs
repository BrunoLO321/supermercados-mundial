
using Mundial.Domain.Entities;
using Xunit;

namespace Mundial.UnitTests;

public class ItemPedidoCompraTests
{
    private static ItemPedidoCompra CriarItem()
    {
        var fornecedor = new Fornecedor(
            "58464982000152",
            "Fornecedor Teste");

        var produto = new Produto(
            "TESTE-001",
            "Produto de Teste",
            10m);

        return new ItemPedidoCompra(
            produto,
            10m,
            8.50m);
    }

    [Fact]
    public void RegistrarRecebimento_ComQuantidadeValida_DeveAtualizarQuantidade()
    {
        // Arrange
        var item = CriarItem();

        // Act
        item.RegistrarRecebimento(4m);

        // Assert
        Assert.Equal(4m, item.QuantidadeRecebida);
        Assert.Equal(6m, item.QuantidadePendente);
    }

    [Fact]
    public void RegistrarRecebimento_AcimaDaQuantidadePendente_DeveLancarExcecao()
    {
        // Arrange
        var item = CriarItem();

        // Act e Assert
        Assert.Throws<InvalidOperationException>(
            () => item.RegistrarRecebimento(11m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RegistrarRecebimento_ComQuantidadeInvalida_DeveLancarExcecao(
        decimal quantidade)
    {
        // Arrange
        var item = CriarItem();
    

        // Act e Assert
        Assert.Throws<ArgumentException>(
            () => item.RegistrarRecebimento(quantidade));
    }
}
