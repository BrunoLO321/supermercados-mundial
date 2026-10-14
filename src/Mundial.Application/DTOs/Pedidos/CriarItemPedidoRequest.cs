namespace Mundial.Application.DTOs.Pedidos;

public record CriarItemPedidoRequest(
    int ProdutoId,
    decimal Quantidade,
    decimal PrecoUnitario);