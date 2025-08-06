using Analytics.Application.DTOs;
using Analytics.Application.Repositories;
using Analytics.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("events")]
public class EventController : ControllerBase
{
    private readonly IEventRepository _eventRepository;

    public EventController(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllEvents()
    {
        List<Event> events = await _eventRepository.GetAllEvents();
        return Ok(events);
    }

    [HttpPost]
    public async Task<IActionResult> NewEvent([FromBody] EventDto eventDto)
    {
        Event newEvent = await _eventRepository.NewEvent(eventDto);
        return Ok(newEvent);
    }
   
}
