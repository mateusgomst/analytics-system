using System;
using System.Collections.Generic;

public class AnalyticsOverviewDto
{
    public PeriodoDto Periodo { get; set; }
    public ResumoDto Resumo { get; set; }
    public TendenciasDto Tendencias { get; set; }
    public List<PaginaMaisVisitadaDto> PaginasMaisVisitadas { get; set; }
}

public class PeriodoDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class ResumoDto
{
    public int TotalPageViews { get; set; }
    public int UsuariosUnicos { get; set; }
    public int SessoesUnicas { get; set; }
    public int MediaPaginasPorSessao { get; set; }
    public int DuracaoMediaSessaoMinutos { get; set; }
    public double TaxaRejeicaoPercent { get; set; }
}

public class TendenciasDto
{
    public double CrescimentoPageViewsPercent { get; set; }
    public double CrescimentoUsuariosPercent { get; set; }
}

public class PaginaMaisVisitadaDto
{
    public string Page { get; set; }
    public int Views { get; set; }
    public int UsuariosUnicos { get; set; }
    public double TempoMedioMinutos { get; set; }
}