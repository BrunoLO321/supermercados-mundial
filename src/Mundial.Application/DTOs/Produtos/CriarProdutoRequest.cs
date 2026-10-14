namespace Mundial.Application.DTOs.Produtos;

public record CriarProdutoRequest(
    string Codigo,
    string Descricao,
    decimal Preco);