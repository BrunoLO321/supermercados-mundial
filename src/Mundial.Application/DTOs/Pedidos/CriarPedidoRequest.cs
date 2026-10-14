namespace Mundial.Application.DTOs.Pedidos;

public record CriarPedidoRequest(
    string Numero,
    int FornecedorId,
    List<CriarItemPedidoRequest> Itens);