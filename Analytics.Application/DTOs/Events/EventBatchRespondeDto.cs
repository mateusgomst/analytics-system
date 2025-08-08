using System;

namespace Analytics.Application.DTOs;

public class EventBatchResponseDto
{
    public int TotalEvents { get; set; }
    public int Accepted { get; set; }
    public int Rejected { get; set; }
    public string BatchId { get; set; }
    public string Message { get; set; } 
    public DateTime ProcessedAt { get; set; }
}