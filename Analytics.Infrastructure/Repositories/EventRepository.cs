using Analytics.Application.DTOs;
using Analytics.Application.Repositories;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsOverviewDto> GetAnalyticsOverview(
        DateTime startDate,
        DateTime endDate,
        string? device = null,
        string? country = null)
    {
        var baseQuery = _context.Events
            .AsNoTracking()
            .Where(e => e.Timestamp >= startDate
                     && e.Timestamp < endDate);

        // Filtros opcionais em JSON
        if (!string.IsNullOrWhiteSpace(device))
        {
            var filter = JsonSerializer.SerializeToDocument(new { device });
            baseQuery = baseQuery.Where(e => EF.Functions.JsonContains(e.Payload, filter));
        }
        if (!string.IsNullOrWhiteSpace(country))
        {
            var filter = JsonSerializer.SerializeToDocument(new { country });
            baseQuery = baseQuery.Where(e => EF.Functions.JsonContains(e.Payload, filter));
        }

        var pageViewsQuery = baseQuery.Where(e => e.EventType == "page_view");

        // Queries sequenciais
        var totalPageViews = await pageViewsQuery.CountAsync();

        var sessoesUnicas = await pageViewsQuery
            .Where(e => e.SessionId != null)
            .Select(e => e.SessionId!)
            .Distinct()
            .CountAsync();

        var usuariosUnicos = await baseQuery
            .Where(e => e.UserId != null)
            .Select(e => e.UserId!)
            .Distinct()
            .CountAsync();

        var sessBounds = await baseQuery
            .Where(e => e.SessionId != null)
            .GroupBy(e => e.SessionId!)
            .Select(g => new
            {
                Start = g.Min(e => e.Timestamp),
                End = g.Max(e => e.Timestamp)
            })
            .ToListAsync();

        double duracaoMediaSessaoMinutosDouble = sessBounds.Count == 0
            ? 0.0
            : sessBounds.Average(s => (s.End - s.Start).TotalMinutes);

        var bounces = await pageViewsQuery
            .Where(e => e.SessionId != null)
            .GroupBy(e => e.SessionId!)
            .Select(g => g.Count())
            .Where(c => c == 1)
            .CountAsync();

        double mediaPaginasPorSessaoDouble = sessoesUnicas == 0 ? 0.0 : (double)totalPageViews / sessoesUnicas;
        int mediaPaginasPorSessao = (int)Math.Round(mediaPaginasPorSessaoDouble, MidpointRounding.AwayFromZero);
        int duracaoMediaSessaoMinutos = (int)Math.Round(duracaoMediaSessaoMinutosDouble, MidpointRounding.AwayFromZero);
        double taxaRejeicaoPercent = sessoesUnicas == 0 ? 0.0 : (double)bounces / sessoesUnicas * 100.0;

        // PÁGINAS MAIS VISITADAS - IMPLEMENTAÇÃO REAL
        var paginasMaisVisitadasDto = await GetPaginasMaisVisitadas(baseQuery);

        return new AnalyticsOverviewDto
        {
            Periodo = new PeriodoDto
            {
                StartDate = startDate,
                EndDate = endDate
            },
            Resumo = new ResumoDto
            {
                TotalPageViews = totalPageViews,
                UsuariosUnicos = usuariosUnicos,
                SessoesUnicas = sessoesUnicas,
                MediaPaginasPorSessao = mediaPaginasPorSessao,
                DuracaoMediaSessaoMinutos = duracaoMediaSessaoMinutos,
                TaxaRejeicaoPercent = Math.Round(taxaRejeicaoPercent, 1)
            },
            Tendencias = new TendenciasDto
            {
                CrescimentoPageViewsPercent = 12.5, // TODO: calcular
                CrescimentoUsuariosPercent = 8.3    // TODO: calcular
            },
            PaginasMaisVisitadas = paginasMaisVisitadasDto
        };
    }

    private async Task<List<PaginaMaisVisitadaDto>> GetPaginasMaisVisitadas(IQueryable<Event> baseQuery)
    {
        // Buscar todos os eventos page_view com payload
        var eventosRaw = await baseQuery
            .Where(e => e.EventType == "page_view" && e.Payload != null)
            .Select(e => new { 
                PayloadJson = e.Payload, // Isso já é um JsonDocument
                UserId = e.UserId,
                SessionId = e.SessionId,
                Timestamp = e.Timestamp
            })
            .ToListAsync();

        if (!eventosRaw.Any())
            return new List<PaginaMaisVisitadaDto>();

        // Processar JSON em memória para extrair a página
        var eventosComPagina = new List<(string Page, string? UserId, string? SessionId, DateTime Timestamp)>();
        
        foreach (var evento in eventosRaw)
        {
            try
            {
                // PayloadJson já é um JsonDocument, não precisa fazer Parse
                if (evento.PayloadJson.RootElement.TryGetProperty("page", out var pageElement))
                {
                    var page = pageElement.GetString();
                    if (!string.IsNullOrEmpty(page))
                    {
                        eventosComPagina.Add((page, evento.UserId, evento.SessionId, evento.Timestamp));
                    }
                }
            }
            catch (Exception)
            {
                // Ignorar erros de acesso ao JSON
                continue;
            }
        }

        if (!eventosComPagina.Any())
            return new List<PaginaMaisVisitadaDto>();

        // Agrupar e calcular estatísticas
        var estatisticasPaginas = eventosComPagina
            .GroupBy(x => x.Page)
            .Select(g => new {
                Page = g.Key,
                Views = g.Count(),
                UsuariosUnicos = g.Where(x => x.UserId != null).Select(x => x.UserId).Distinct().Count()
            })
            .OrderByDescending(x => x.Views)
            .Take(10) // Top 10 páginas
            .ToList();

        // Calcular tempo médio por página
        var tempoMedioPorPagina = CalcularTempoMedioPorPaginaEmMemoria(eventosComPagina, estatisticasPaginas.Select(p => p.Page).ToList());

        return estatisticasPaginas.Select(p => new PaginaMaisVisitadaDto
        {
            Page = p.Page,
            Views = p.Views,
            UsuariosUnicos = p.UsuariosUnicos,
            TempoMedioMinutos = tempoMedioPorPagina.GetValueOrDefault(p.Page, 0)
        }).ToList();
    }

    private Dictionary<string, double> CalcularTempoMedioPorPaginaEmMemoria(
        List<(string Page, string? UserId, string? SessionId, DateTime Timestamp)> eventos, 
        List<string> paginas)
    {
        var tempoMedioPorPagina = new Dictionary<string, double>();
        
        if (!paginas.Any() || !eventos.Any()) return tempoMedioPorPagina;
        
        var temposPorPagina = new Dictionary<string, List<double>>();
        
        // Agrupar por sessão e calcular tempo entre page views
        foreach (var sessaoGroup in eventos.Where(e => e.SessionId != null).GroupBy(e => e.SessionId))
        {
            var eventosOrdenados = sessaoGroup
                .OrderBy(e => e.Timestamp)
                .ToList();
            
            // Calcular tempo entre páginas consecutivas na mesma sessão
            for (int i = 0; i < eventosOrdenados.Count - 1; i++)
            {
                var paginaAtual = eventosOrdenados[i].Page;
                var proximoEvento = eventosOrdenados[i + 1];
                
                var tempoMinutos = (proximoEvento.Timestamp - eventosOrdenados[i].Timestamp).TotalMinutes;
                
                // Considerar apenas tempos razoáveis (entre 0.1 min e 30 min)
                if (tempoMinutos >= 0.1 && tempoMinutos <= 30)
                {
                    if (!temposPorPagina.ContainsKey(paginaAtual))
                        temposPorPagina[paginaAtual] = new List<double>();
                    
                    temposPorPagina[paginaAtual].Add(tempoMinutos);
                }
            }
        }
        
        // Calcular média para cada página
        foreach (var kvp in temposPorPagina)
        {
            if (kvp.Value.Count > 0)
            {
                tempoMedioPorPagina[kvp.Key] = Math.Round(kvp.Value.Average(), 1);
            }
        }
        
        return tempoMedioPorPagina;
    }

    public async Task<Event> NewEvent(Event newEvent)
    {
        await _context.Events.AddAsync(newEvent);
        await _context.SaveChangesAsync();
        return newEvent;
    }

    public async Task<List<Event>> GetAllEvents()
    {
        return await _context.Events
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();
    }
}