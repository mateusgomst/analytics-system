using Analytics.Application.DTOs;
using Analytics.Application.Repositories;
using Analytics.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

public class VendaRepository : IVendaRepository
{
    private readonly ApplicationDbContext _context;

    public VendaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RelatorioMensalDto>> ObterRelatorioMensalAsync(int ano)
    {
        return await _context.Vendas
            .Where(v => v.Data.Year == ano)
            .GroupBy(v => v.Data.Month)
            .Select(g => new RelatorioMensalDto
            {
                Mes = new DateTime(ano, g.Key, 1).ToString("MMMM"),
                TotalVendas = g.Sum(v => v.Valor)
            })
            .ToListAsync();
    }
}
