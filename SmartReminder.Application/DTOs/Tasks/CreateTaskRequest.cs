using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.DTOs.Tasks;

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueAtUtc { get; set; }

    public ReminderPriority Priority { get; set; } = ReminderPriority.Medium;

    public List<string> Steps { get; set; } = new();
}