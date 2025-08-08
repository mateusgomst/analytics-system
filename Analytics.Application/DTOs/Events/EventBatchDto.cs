using System.Collections.Generic;

namespace Analytics.Application.DTOs;

public class EventBatchDto
{
    public List<EventDto> Events { get; set; }
}