using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Analytics.Domain.Entities
{
    public class AggregatedMetric
    {
        public Guid Id { get; set; }
        public string MetricName { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public JsonDocument Dimensions { get; set; }
        public string TenantId { get; set; }
        public DateTime UpdatedAt { get; set; }

        public AggregatedMetric(string metricName, double value, JsonDocument dimensions, string tenantId)
        {
            Id = Guid.NewGuid();
            MetricName = metricName;
            Date = DateTime.UtcNow.Date; 
            Value = value;
            Dimensions = dimensions;
            TenantId = tenantId;
            UpdatedAt = DateTime.UtcNow;
        }

        private AggregatedMetric() { }
    }
}