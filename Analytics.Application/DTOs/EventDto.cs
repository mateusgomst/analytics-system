namespace Analytics.Application.DTOs;

public class EventDto
{
    public string EventType { get; set; }
    public Dictionary<string, Object> Payload { get; set; } = new();
    public string UserId { get; set; }
}
