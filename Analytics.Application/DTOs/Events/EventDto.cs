using System.Text.Json;

namespace Analytics.Application.DTOs;

public class EventDto
{
    public string EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public string SessionId { get; set; }
    public JsonDocument Payload { get; set; }
}

