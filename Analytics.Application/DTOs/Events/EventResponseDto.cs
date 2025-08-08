using System.Text.Json;

namespace Analytics.Application.DTOs;

public class EventResponseDto
{

    //   "event_id": "550e8400-e29b-41d4-a716-446655440000",
    //   "status": "accepted",
    //   "processed_at": "2025-08-07T17:25:19Z"

    public string EventId { get; set; }
    public string Status { get; set; }
    public DateTime ProcessedAt { get; set; }

    public EventResponseDto(string eventId, string status, DateTime processedAt)
    {
        EventId = eventId;
        Status = status;
        ProcessedAt = processedAt;
    }


}

