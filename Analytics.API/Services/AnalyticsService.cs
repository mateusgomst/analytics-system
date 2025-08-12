using Analytics.Application.Repositories;

namespace Analytics.API.Services;

public class AnalyticsService
{
    private readonly IEventRepository _eventRepository;

    public AnalyticsService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<AnalyticsOverviewDto> GetAnalyticsOverview(DateTime startDate, DateTime endDate, string? device = null, string? country = null)
    {
        AnalyticsOverviewDto response = await _eventRepository.GetAnalyticsOverview(
            startDate,
            endDate,
            device,
            country);

        return response;
    }

       

}

