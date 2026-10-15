
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


    [HttpGet]
    public async Task<IActionResult> Listar(
        CancellationToken cancellationToken)
    {
        var fornecedores = await _fornecedorService.ListarAsync(
            cancellationToken);

        var resultado = fornecedores.Select(f => new
        {
            f.Id,
            f.Cnpj,
            f.RazaoSocial,
            f.Ativo
        });

        return Ok(resultado);
    }


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
