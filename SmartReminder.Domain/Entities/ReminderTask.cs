using SmartReminder.Domain.Common;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Domain.Entities;

public class ReminderTask : BaseEntity
{
    public int OwnerUserId { get; set; }

    public int CreatedByUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueAtUtc { get; set; }

    public ReminderPriority Priority { get; set; } = ReminderPriority.Medium;

    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;

    public int MissedCount { get; set; }

    public bool IsSuggestedByParentOrTeacher { get; set; }

    public bool RequiresStudentApproval { get; set; }

    public int? SourceChatMessageId { get; set; }

    public AppUser OwnerUser { get; set; } = null!;

    public AppUser CreatedByUser { get; set; } = null!;

    public ChatMessage? SourceChatMessage { get; set; }

    public ICollection<TaskStep> Steps { get; set; } = new List<TaskStep>();

    public ICollection<PomodoroSession> PomodoroSessions { get; set; } = new List<PomodoroSession>();

    public ICollection<VisualScheduleItem> VisualScheduleItems { get; set; } = new List<VisualScheduleItem>();
}