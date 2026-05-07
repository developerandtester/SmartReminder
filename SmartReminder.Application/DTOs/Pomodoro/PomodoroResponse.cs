using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.DTOs.Pomodoro;

public class PomodoroResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? ReminderTaskId { get; set; }

    public string? LinkedTaskTitle { get; set; }

    public int FocusDurationMinutes { get; set; }

    public int ShortBreakMinutes { get; set; }

    public int LongBreakMinutes { get; set; }

    public int CycleNumber { get; set; }

    public PomodoroStatus Status { get; set; }

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? PausedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public string? Notes { get; set; }
}