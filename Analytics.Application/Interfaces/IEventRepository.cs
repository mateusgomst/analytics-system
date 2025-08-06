using System.Net;
using Analytics.Application.DTOs;
using Analytics.Domain.Entities;

namespace Analytics.Application.Repositories;

public interface IEventRepository
{
    Task<Event> NewEvent(EventDto eventDto);
    Task<List<Event>> GetAllEvents();

}
