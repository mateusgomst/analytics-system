using Analytics.API.Services;
using Analytics.Application.Constants;
using Analytics.Application.DTOs;
using Analytics.Application.Interfaces;
using Analytics.Application.Repositories;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;

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
    }

   [HttpPost("batch")]
    public async Task<IActionResult> NewEventsBatch([FromBody] EventBatchDto batch, [FromHeader(Name = "X-Tenant-ID")] string tenantId)
    {
        var response = new EventBatchResponseDto
        {
            TotalEvents = batch.Events.Count,
            Accepted = 0,
            Rejected = 0,
            BatchId = Guid.NewGuid().ToString(),
            ProcessedAt = DateTime.UtcNow,
            Message = "Batch processed"
        };

        foreach (var eventDto in batch.Events.Take(100))
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
                response.Accepted++;
            }
            catch (Exception)
            {
                response.Rejected++;
            }
        }

        var exceeded = batch.Events.Count - 100;
        if (exceeded > 0)
        {
            response.Rejected += exceeded; 
            response.Message = "All events processed successfully, but only the first 100 were accepted. The remaining events were rejected for exceeding the limit of 100 events per request.";
        }
        else if (response.Rejected > 0)
        {
            response.Message = "Some events were not accepted due to processing errors.";
        }

        return Accepted(response);
    }
}
