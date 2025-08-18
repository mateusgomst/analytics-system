using System.Threading.Tasks;
using Analytics.API.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/v1/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analyticsService;
    public AnalyticsController(AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetAnalyticsOverview(
        [FromQuery] string start_date,
        [FromQuery] string end_date,
        [FromQuery] string? device = null,
        [FromQuery] string? country = null)
    {
        try
        {
            var startDate = DateTime.SpecifyKind(
                DateTime.ParseExact(start_date, "dd/MM/yyyy", null),
                DateTimeKind.Utc
            );
            var endDate = DateTime.SpecifyKind(
                DateTime.ParseExact(end_date, "dd/MM/yyyy", null),
                DateTimeKind.Utc
            );
            var overview = await _analyticsService.GetAnalyticsOverview(
                startDate,
                endDate,
                device,
                country);
            return Ok(overview);
        }
        catch (FormatException)
        {
            return BadRequest("Formato de data inválido. Use dd/MM/yyyy.");
        }
    }
}
