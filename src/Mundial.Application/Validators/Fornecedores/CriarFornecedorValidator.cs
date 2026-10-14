using FluentValidation;
using Mundial.Application.DTOs.Fornecedores;

namespace Mundial.Application.Validators.Fornecedores;

public class CriarFornecedorValidator
    : AbstractValidator<CriarFornecedorRequest>
{
    public CriarFornecedorValidator()
    {
        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .Length(14);

        RuleFor(x => x.RazaoSocial)
            .NotEmpty()
            .MaximumLength(200);
    }
}