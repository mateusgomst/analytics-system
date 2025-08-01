using Analytics.Application.DTOs;

namespace Analytics.Application.Repositories;

public interface IVendaRepository
{
    Task<List<RelatorioMensalDto>> ObterRelatorioMensalAsync(int ano);
}
