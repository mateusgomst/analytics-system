namespace Analytics.Domain.Entities;

public class AggregatedMetric
{
    //Id, MetricName, Timestamp, Value, Filters
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MetricName { get; set; }
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
    public Dictionary<string, string> Filters { get; set; }
    
}