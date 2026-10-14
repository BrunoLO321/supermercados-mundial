namespace Mundial.Domain.Entities;

public class ItemDocumentoFiscal
{
    public int Id { get; private set; }

    public int DocumentoFiscalId { get; private set; }

    public DocumentoFiscal DocumentoFiscal { get; private set; } = null!;

    public int ProdutoId { get; private set; }

    public Produto Produto { get; private set; } = null!;

    public decimal Quantidade { get; private set; }

    public decimal PrecoUnitario { get; private set; }

    private ItemDocumentoFiscal()
    {
    }

    public ItemDocumentoFiscal(
        Produto produto,
        decimal quantidade,
        decimal precoUnitario)
    {
        Produto = produto;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }
}