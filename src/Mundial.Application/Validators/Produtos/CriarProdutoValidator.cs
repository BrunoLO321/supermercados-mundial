using FluentValidation;
using Mundial.Application.DTOs.Produtos;

namespace Mundial.Application.Validators.Produtos;

public class CriarProdutoValidator
    : AbstractValidator<CriarProdutoRequest>
{
    public CriarProdutoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Preco)
            .GreaterThanOrEqualTo(0);
    }
}