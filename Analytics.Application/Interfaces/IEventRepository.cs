using System.Net;
using Analytics.Application.DTOs;
using Analytics.Domain.Entities;

namespace Analytics.Application.Repositories;

public interface IEventRepository
{
    Task<Event> NewEvent(Event newEvent);
    Task<List<Event>> GetAllEvents();
    Task<AnalyticsOverviewDto> GetAnalyticsOverview(DateTime startDate, DateTime endDate, string? device = null, string? country = null);

}
