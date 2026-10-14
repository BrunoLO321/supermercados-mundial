using Mundial.Domain.Enums;

namespace Mundial.Domain.Entities;

public class AcordoComercial
{
    public int Id { get; private set; }

    public int PedidoCompraId { get; private set; }

    public int ProdutoId { get; private set; }

    public decimal ValorDivergencia { get; private set; }

    public string Motivo { get; private set; } = string.Empty;

    public StatusAcordoComercial Status { get; private set; }

    public DateTime DataCriacao { get; private set; }

    public PedidoCompra PedidoCompra { get; private set; } = null!;

    public Produto Produto { get; private set; } = null!;

    private AcordoComercial()
    {
    }

    public AcordoComercial(
        int pedidoCompraId,
        int produtoId,
        decimal valorDivergencia,
        string motivo)
    {
        if (valorDivergencia <= 0)
            throw new ArgumentException(
                "O valor da divergência deve ser maior que zero.");

        PedidoCompraId = pedidoCompraId;
        ProdutoId = produtoId;
        ValorDivergencia = valorDivergencia;
        Motivo = motivo;
        Status = StatusAcordoComercial.Pendente;
        DataCriacao = DateTime.UtcNow;
    }
}