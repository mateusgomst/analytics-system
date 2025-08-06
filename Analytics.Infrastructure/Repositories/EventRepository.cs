using Analytics.Application.DTOs;
using Analytics.Application.Repositories;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Context;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Event> NewEvent(EventDto eventDto)
    {
        Event newEvent = new Event(eventDto.EventType, eventDto.Payload, eventDto.UserId);
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