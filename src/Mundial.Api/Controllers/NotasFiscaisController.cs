
using Microsoft.AspNetCore.Mvc;
using Mundial.Application.Exceptions;
using Mundial.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Mundial.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/notas-fiscais")]
public class NotasFiscaisController : ControllerBase
{
    private readonly RecebimentoFiscalService _recebimentoFiscalService;

    public NotasFiscaisController(
        RecebimentoFiscalService recebimentoFiscalService)
    {
        _recebimentoFiscalService = recebimentoFiscalService;
    }

    [HttpPost("importar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Importar(
        IFormFile arquivo,
        CancellationToken cancellationToken)
    {
        if (arquivo is null || arquivo.Length == 0)
        {
            return BadRequest(new
            {
                mensagem = "Selecione um arquivo XML válido."
            });
        }

        if (!Path.GetExtension(arquivo.FileName)
            .Equals(".xml", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                mensagem = "O arquivo enviado deve possuir extensão XML."
            });
        }

        try
        {
            await using var stream = arquivo.OpenReadStream();

            await _recebimentoFiscalService.ProcessarXmlAsync(
                stream,
                cancellationToken);

            return Ok(new
            {
                mensagem = "Nota fiscal importada com sucesso."
            });
        }
        catch (RegraNegocioException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }
}
