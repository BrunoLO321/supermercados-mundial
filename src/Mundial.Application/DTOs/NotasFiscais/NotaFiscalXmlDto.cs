
namespace Mundial.Application.DTOs.NotasFiscais;

public class NotaFiscalXmlDto
{
    public string ChaveAcesso { get; set; } = string.Empty;

    public string CnpjEmitente { get; set; } = string.Empty;

    public DateTime DataEmissao { get; set; }

    public string NumeroPedido { get; set; } = string.Empty;

    public List<ItemNotaFiscalXmlDto> Itens { get; set; } = new();
}

public class ItemNotaFiscalXmlDto
{
    public string CodigoProduto { get; set; } = string.Empty;

    public decimal Quantidade { get; set; }

    public decimal Preco { get; set; }
}
