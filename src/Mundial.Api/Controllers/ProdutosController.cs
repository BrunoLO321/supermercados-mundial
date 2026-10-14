
using Microsoft.AspNetCore.Mvc;
using Mundial.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Mundial.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly ProdutoService _produtoService;

    public ProdutosController(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    public record CriarProdutoRequest(
        string Codigo,
        string Descricao,
        decimal Preco);


    [HttpGet]
    public async Task<IActionResult> ObterTodos(
        CancellationToken cancellationToken)
    {
        var produtos = await _produtoService.ObterTodosAsync(
            cancellationToken);

        var resultado = produtos.Select(p => new
        {
            p.Id,
            p.Codigo,
            p.Descricao,
            p.Preco,
            Status = p.Status.ToString(),
            Estoque = p.Estoque == null ? null : new
            {
                p.Estoque.Quantidade
            }
        });

        return Ok(resultado);
    }


    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarProdutoRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _produtoService.CriarAsync(
            request.Codigo,
            request.Descricao,
            request.Preco,
            cancellationToken);

        return Created(
            $"/api/produtos/{id}",
            new
            {
                id,
                mensagem = "Produto cadastrado com sucesso."
            });
    }
}
