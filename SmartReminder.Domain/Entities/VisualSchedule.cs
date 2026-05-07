using SmartReminder.Domain.Common;

namespace SmartReminder.Domain.Entities;

public class VisualSchedule : BaseEntity
{
    public int StudentId { get; set; }

    public int CreatedByUserId { get; set; }

    public DateOnly ScheduleDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public AppUser Student { get; set; } = null!;

    public AppUser CreatedByUser { get; set; } = null!;

    public ICollection<VisualScheduleItem> Items { get; set; } = new List<VisualScheduleItem>();
}