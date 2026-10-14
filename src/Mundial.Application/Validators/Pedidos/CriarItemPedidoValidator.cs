using FluentValidation;
using Mundial.Application.DTOs.Pedidos;

namespace Mundial.Application.Validators.Pedidos;

public class CriarItemPedidoValidator
    : AbstractValidator<CriarItemPedidoRequest>
{
    public CriarItemPedidoValidator()
    {
        RuleFor(x => x.ProdutoId)
            .GreaterThan(0);

        RuleFor(x => x.Quantidade)
            .GreaterThan(0);

        RuleFor(x => x.PrecoUnitario)
            .GreaterThanOrEqualTo(0);
    }
}