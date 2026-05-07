namespace SmartReminder.Application.DTOs.Pomodoro;

public class CompletePomodoroRequest
{
    public string? Notes { get; set; }

    public bool MarkLinkedTaskCompleted { get; set; }
}