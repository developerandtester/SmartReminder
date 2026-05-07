using SmartReminder.Domain.Common;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Domain.Entities;

public class PomodoroSession : BaseEntity
{
    public int UserId { get; set; }

    public int? ReminderTaskId { get; set; }

    public int FocusDurationMinutes { get; set; } = 25;

    public int ShortBreakMinutes { get; set; } = 5;

    public int LongBreakMinutes { get; set; } = 15;

    public int CycleNumber { get; set; } = 1;

    public PomodoroStatus Status { get; set; } = PomodoroStatus.NotStarted;

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? PausedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public string? Notes { get; set; }

    public AppUser User { get; set; } = null!;

    public ReminderTask? ReminderTask { get; set; }
}