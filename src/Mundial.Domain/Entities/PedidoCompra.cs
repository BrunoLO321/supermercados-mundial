using Mundial.Domain.Enums;

namespace Mundial.Domain.Entities;

public class PedidoCompra
{
    public int Id { get; private set; }

    public string Numero { get; private set; } = string.Empty;

    public DateTime DataEmissao { get; private set; }

    public StatusPedido Status { get; private set; }

    public int FornecedorId { get; private set; }

    public Fornecedor Fornecedor { get; private set; } = null!;

    public ICollection<ItemPedidoCompra> Itens { get; private set; }
        = new List<ItemPedidoCompra>();

    public ICollection<DocumentoFiscal> DocumentosFiscais { get; private set; }
        = new List<DocumentoFiscal>();

    private PedidoCompra()
    {
    }

    public PedidoCompra(
        string numero,
        Fornecedor fornecedor)
    {
        Numero = numero;
        Fornecedor = fornecedor;
        DataEmissao = DateTime.UtcNow;
        Status = StatusPedido.Aberto;
    }

    public void AdicionarItem(
        Produto produto,
        decimal quantidade,
        decimal precoUnitario)
    {
        if (Status != StatusPedido.Aberto)
            throw new InvalidOperationException(
                "Não é possível adicionar itens a um pedido que não está aberto.");

        Itens.Add(
            new ItemPedidoCompra(
                produto,
                quantidade,
                precoUnitario));
    }

    public void AtualizarStatus()
    {
        if (!Itens.Any())
        {
            Status = StatusPedido.Aberto;
            return;
        }

        var pedidoTotalmenteRecebido = Itens.All(
            x => x.QuantidadeRecebida >= x.QuantidadeSolicitada);

        if (pedidoTotalmenteRecebido)
        {
            Status = StatusPedido.Fechado;
            return;
        }

        var possuiRecebimento = Itens.Any(
            x => x.QuantidadeRecebida > 0);

        Status = possuiRecebimento
            ? StatusPedido.Parcial
            : StatusPedido.Aberto;
    }
}