using System.Text.Json;

namespace Analytics.Domain.Entities;

public class AlertRule
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string MetricName { get; set; }
    public JsonDocument Condition { get; set; }
    public string NotificationUrl { get; set; }
    public bool IsActive { get; set; }
    public string TenantId { get; set; }
    public DateTime CreatedAt { get; set; }

    public AlertRule(string name, string metricName, JsonDocument condition, string notificationUrl, bool isActive, string tenantId)
    {
        Id = Guid.NewGuid();
        Name = name;
        MetricName = metricName;
        Condition = condition;
        NotificationUrl = notificationUrl;
        IsActive = isActive;
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;
    }

    // Construtor sem parâmetros para EF Core
    private AlertRule() { }
}