namespace SmartReminder.Application.DTOs.Pomodoro;

public class StartPomodoroRequest
{
    public int? ReminderTaskId { get; set; }

    public int FocusDurationMinutes { get; set; } = 25;

    public int ShortBreakMinutes { get; set; } = 5;

    public int LongBreakMinutes { get; set; } = 15;

    public int CycleNumber { get; set; } = 1;
}