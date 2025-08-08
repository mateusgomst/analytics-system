using System.Data;
using System.Data.Common;
using System.Text.Json;

namespace Analytics.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public string SessionId { get; set; }
    public JsonDocument Payload { get; set; }
    public string TenantId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Event(string eventType, JsonDocument payload, string userId, DateTime? timestamp = null, string sessionId = null, string tenantId = null)
    {
        Id = Guid.NewGuid();
        EventType = eventType;
        Timestamp = timestamp ?? DateTime.UtcNow; 
        Payload = payload;
        UserId = userId;
        SessionId = sessionId ?? string.Empty;
        TenantId = tenantId ?? "default-tenant";
        CreatedAt = DateTime.UtcNow;
    }

    private Event() { }
}