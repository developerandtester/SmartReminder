using SmartReminder.Domain.Common;

namespace SmartReminder.Domain.Entities;

public class TaskStep : BaseEntity
{
    public int ReminderTaskId { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int SortOrder { get; set; }

    public ReminderTask ReminderTask { get; set; } = null!;
}