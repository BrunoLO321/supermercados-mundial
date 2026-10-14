
using Microsoft.AspNetCore.Mvc;
using Mundial.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Mundial.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/fornecedores")]
public class FornecedoresController : ControllerBase
{
    private readonly FornecedorService _fornecedorService;

    public FornecedoresController(
        FornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService;
    }

    public record CriarFornecedorRequest(
        string Cnpj,
        string RazaoSocial);

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarFornecedorRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _fornecedorService.CriarAsync(
            request.Cnpj,
            request.RazaoSocial,
            cancellationToken);

        return Created(
            $"/api/fornecedores/{id}",
            new { id, mensagem = "Fornecedor cadastrado com sucesso." });
    }
}
