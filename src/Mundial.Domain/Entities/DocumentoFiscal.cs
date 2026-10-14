namespace Mundial.Domain.Entities;

public class DocumentoFiscal
{
    public int Id { get; private set; }

    public string ChaveAcesso { get; private set; } = string.Empty;

    public string CnpjEmitente { get; private set; } = string.Empty;

    public DateTime DataEmissao { get; private set; }

    public int PedidoCompraId { get; private set; }

    public PedidoCompra PedidoCompra { get; private set; } = null!;

    public ICollection<ItemDocumentoFiscal> Itens { get; private set; }
        = new List<ItemDocumentoFiscal>();

    private DocumentoFiscal()
    {
    }

    public DocumentoFiscal(
        string chaveAcesso,
        string cnpjEmitente,
        DateTime dataEmissao,
        PedidoCompra pedidoCompra)
    {
        ChaveAcesso = chaveAcesso;
        CnpjEmitente = cnpjEmitente;
        DataEmissao = dataEmissao;
        PedidoCompra = pedidoCompra;
    }
}