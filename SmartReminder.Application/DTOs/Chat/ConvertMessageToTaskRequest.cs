namespace SmartReminder.Application.DTOs.Chat;

public class ConvertMessageToTaskRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDateUtc { get; set; }

    public int EstimatedPomodoroSessions { get; set; } = 1;

    public bool HighPriority { get; set; }
}