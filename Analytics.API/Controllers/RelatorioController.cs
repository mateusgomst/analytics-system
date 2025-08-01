using Analytics.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("relatorios")]
public class RelatorioController : ControllerBase
{
    private readonly IVendaRepository _vendaRepository;

    public RelatorioController(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    [HttpGet("mensal")]
    public async Task<IActionResult> GetMensal([FromQuery] int ano)
    {
        var relatorio = await _vendaRepository.ObterRelatorioMensalAsync(ano);
        return Ok(relatorio);
    }
}
