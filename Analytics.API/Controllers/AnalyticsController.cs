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
    public IActionResult GetAnalyticsOverview(
        [FromQuery] DateTime start_date,
        [FromQuery] DateTime end_date,
        [FromQuery] string? device = null,
        [FromQuery] string? country = null)
    {
        var overview = _analyticsService.GetAnalyticsOverview();
        return Ok(overview);
    }
}
