using Analytics.API.Services;
using Analytics.Application.Constants;
using Analytics.Application.DTOs;
using Analytics.Application.Interfaces;
using Analytics.Application.Repositories;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/v1/events")]
public class EventController : ControllerBase
{
    private readonly RabbitMqMessageBus _rabbitMqMessageBus;

    private readonly IMessageBus _messageBus;

    public EventController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost]
    public async Task<IActionResult> NewEvent([FromBody] EventDto eventDto, [FromHeader(Name = "X-Tenant-ID")] string tenantId)
    {
        try
        {
            Event newEvent = new Event(
                eventDto.EventType,
                eventDto.Payload,
                eventDto.UserId,
                eventDto.Timestamp,
                eventDto.SessionId,
                tenantId);

            await _messageBus.PublishAsync(QueueNames.Events, newEvent);
            return Accepted(new EventResponseDto(newEvent.Id.ToString(), "accepted", newEvent.CreatedAt));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "Invalid request", details = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { error = "Service unavailable", details = "Message queue is not available" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
   
}
