using System.Net;
using Analytics.Application.DTOs;

namespace Analytics.Application.Repositories;

public interface IVendaRepository
{
    Task<List<RelatorioMensalDto>> ObterRelatorioMensalAsync(int ano);
    Task<VendaDto> AdcionarNovaVenda(VendaDto vendaDto);
}
