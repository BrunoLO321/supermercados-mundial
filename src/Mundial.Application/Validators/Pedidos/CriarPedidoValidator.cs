using FluentValidation;
using Mundial.Application.DTOs.Pedidos;

namespace Mundial.Application.Validators.Pedidos;

public class CriarPedidoValidator
    : AbstractValidator<CriarPedidoRequest>
{
    public CriarPedidoValidator()
    {
        RuleFor(x => x.Numero)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.FornecedorId)
            .GreaterThan(0);

        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("O pedido deve possuir pelo menos um item.");

        RuleForEach(x => x.Itens)
            .SetValidator(new CriarItemPedidoValidator());
    }
}