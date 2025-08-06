using System.Data;
using System.Data.Common;

namespace Analytics.Domain.Entities;

public class Event
{
    //Id, EventType, Timestamp, Payload, UserId
    public Event(string eventType, string payload, string userId)
    {
        this.EventType = eventType;
        this.Timestamp = DateTime.UtcNow;
        this.Payload = payload;
        this.UserId = userId;
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public string Payload { get; set; }
    public string UserId { get; set; }


}