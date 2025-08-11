namespace Analytics.API.Services;

public class AnalyticsService
{
    public AnalyticsOverviewDto GetAnalyticsOverview()
    {
        AnalyticsOverviewDto response = new AnalyticsOverviewDto
        {
            Periodo = new PeriodoDto
            {
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow
            },
            Resumo = new ResumoDto
            {
                TotalPageViews = 50000,
                UsuariosUnicos = 12000,
                SessoesUnicas = 8000,
                MediaPaginasPorSessao = 6.25,
                DuracaoMediaSessaoMinutos = 4.8,
                TaxaRejeicaoPercent = 38.2
            },
            Tendencias = new TendenciasDto
            {
                CrescimentoPageViewsPercent = 12.5,
                CrescimentoUsuariosPercent = 8.3
            },
            PaginasMaisVisitadas = new List<PaginaMaisVisitadaDto>
            {
                new PaginaMaisVisitadaDto
                {
                    Page = "/home",
                    Views = 15000,
                    UsuariosUnicos = 7000,
                    TempoMedioMinutos = 3.2
                },
                new PaginaMaisVisitadaDto
                {
                    Page = "/produtos",
                    Views = 12000,
                    UsuariosUnicos = 5000,
                    TempoMedioMinutos = 2.7
                },
                new PaginaMaisVisitadaDto
                {
                        Page = "/contato",
                        Views = 8000,
                        UsuariosUnicos = 3000,
                        TempoMedioMinutos = 1.9
                    }
                }
        };

        return response;
    }

}

