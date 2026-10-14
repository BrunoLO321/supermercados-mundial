using Mundial.Domain.Enums;

namespace Mundial.Domain.Entities;

public class Produto
{
    public int Id { get; private set; }

    public string Codigo { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;

    public decimal Preco { get; private set; }

    public StatusProduto Status { get; private set; }

    public Estoque? Estoque { get; private set; }

    public ICollection<ItemPedidoCompra> ItensPedido { get; private set; }
        = new List<ItemPedidoCompra>();

    public ICollection<ItemDocumentoFiscal> ItensDocumentoFiscal { get; private set; }
        = new List<ItemDocumentoFiscal>();

    private Produto()
    {
    }

  
    public Produto(string codigo, string descricao, decimal preco)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("O código do produto é obrigatório.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição do produto é obrigatória.");

        if (preco < 0)
            throw new ArgumentException("O preço não pode ser negativo.");

        Codigo = codigo;
        Descricao = descricao;
        Preco = preco;
        Status = StatusProduto.Ativo;

        Estoque = new Estoque(this);
    }


    public void AtualizarStatus(StatusProduto status)
    {
        Status = status;
    }

    public bool PermiteCompra()
    {
        return Status == StatusProduto.Ativo;
    }

    public bool PermiteRecebimento()
    {
        return Status != StatusProduto.Inativo;
    }
}