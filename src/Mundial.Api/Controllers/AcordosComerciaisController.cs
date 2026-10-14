
using Microsoft.AspNetCore.Mvc;
using Mundial.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Mundial.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/acordos-comerciais")]
public class AcordosComerciaisController : ControllerBase
{
    private readonly IAcordoComercialRepository _repository;

    public AcordosComerciaisController(
        IAcordoComercialRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos(
        CancellationToken cancellationToken)
    {
        var acordos = await _repository.ObterTodosAsync(
            cancellationToken);

        return Ok(acordos.Select(a => new
        {
            a.Id,
            a.PedidoCompraId,
            a.ProdutoId,
            a.ValorDivergencia,
            a.Motivo,
            Status = a.Status.ToString(),
            a.DataCriacao
        }));
    }
}
