using Analytics.Application.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Analytics.API.Services;

public class AnalyticsService
{
    private readonly IEventRepository _eventRepository;
    private readonly IMemoryCache _cache;

    public AnalyticsService(IEventRepository eventRepository, IMemoryCache cache)
    {
        _eventRepository = eventRepository;
        _cache = cache;
    }

    public async Task<AnalyticsOverviewDto> GetAnalyticsOverview(DateTime startDate, DateTime endDate, string? device = null, string? country = null)
    {
        string cacheKey = $"AnalyticsOverview:{startDate:yyyyMMddHHmmss}:{endDate:yyyyMMddHHmmss}:{device}:{country}";
        if (_cache.TryGetValue(cacheKey, out AnalyticsOverviewDto cachedOverview))
        {
            return cachedOverview;
        }

        AnalyticsOverviewDto response = await _eventRepository.GetAnalyticsOverview(
            startDate,
            endDate,
            device,
            country);

        _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10)); // Expira em 10 minutos

        return response;
    }
}
