namespace Mundial.Application.DTOs.Fornecedores;

public record CriarFornecedorRequest(
    string Cnpj,
    string RazaoSocial);