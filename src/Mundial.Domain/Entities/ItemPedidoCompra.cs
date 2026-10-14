namespace Mundial.Domain.Entities;

public class ItemPedidoCompra
{
    public int Id { get; private set; }

    public int PedidoCompraId { get; private set; }

    public PedidoCompra PedidoCompra { get; private set; } = null!;

    public int ProdutoId { get; private set; }

    public Produto Produto { get; private set; } = null!;

    public decimal QuantidadeSolicitada { get; private set; }

    public decimal QuantidadeRecebida { get; private set; }

    public decimal PrecoUnitario { get; private set; }

    public decimal QuantidadePendente =>
        QuantidadeSolicitada - QuantidadeRecebida;

    private ItemPedidoCompra()
    {
    }

    public ItemPedidoCompra(
        Produto produto,
        decimal quantidadeSolicitada,
        decimal precoUnitario)
    {
        if (quantidadeSolicitada <= 0)
            throw new ArgumentException(
                "A quantidade deve ser maior que zero.");

        if (precoUnitario < 0)
            throw new ArgumentException(
                "O preço não pode ser negativo.");

        Produto = produto;
        QuantidadeSolicitada = quantidadeSolicitada;
        PrecoUnitario = precoUnitario;
    }

    public void RegistrarRecebimento(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException(
                "A quantidade recebida deve ser maior que zero.");

        if (quantidade > QuantidadePendente)
            throw new InvalidOperationException(
                "A quantidade recebida é superior à quantidade pendente.");

        QuantidadeRecebida += quantidade;
    }
}