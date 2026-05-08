using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.DTOs.VisualSchedules;

public class CreateVisualScheduleItemRequest
{
    public int? LinkedTaskId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? IconName { get; set; }

    public string? ColorCode { get; set; }

    public ScheduleItemType ItemType { get; set; } = ScheduleItemType.Custom;

    public int SortOrder { get; set; }
}