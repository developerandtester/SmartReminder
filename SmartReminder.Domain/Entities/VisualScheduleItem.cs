using SmartReminder.Domain.Common;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Domain.Entities;

public class VisualScheduleItem : BaseEntity
{
    public int VisualScheduleId { get; set; }

    public int? LinkedTaskId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? IconName { get; set; }

    public string? ColorCode { get; set; }

    public ScheduleItemType ItemType { get; set; } = ScheduleItemType.Custom;

    public int SortOrder { get; set; }

    public VisualSchedule VisualSchedule { get; set; } = null!;

    public ReminderTask? LinkedTask { get; set; }
}