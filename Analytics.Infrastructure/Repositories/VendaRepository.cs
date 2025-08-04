using Analytics.Application.DTOs;
using Analytics.Application.Repositories;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Context;
using FluentValidation.Validators;
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

    public async Task<VendaDto> AdcionarNovaVenda(VendaDto novaVenda)
    {
        Venda venda = new Venda(novaVenda.Valor, novaVenda.Produto);
        await _context.Vendas.AddAsync(venda);
        await _context.SaveChangesAsync();
        return novaVenda;
    }
}
