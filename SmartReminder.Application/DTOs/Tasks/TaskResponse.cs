using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.DTOs.Tasks;

public class TaskResponse
{
    public int Id { get; set; }

    public int OwnerUserId { get; set; }

    public int CreatedByUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueAtUtc { get; set; }

    public ReminderPriority Priority { get; set; }

    public ReminderStatus Status { get; set; }

    public int MissedCount { get; set; }

    public bool IsSuggestedByParentOrTeacher { get; set; }

    public bool RequiresStudentApproval { get; set; }

    public List<TaskStepResponse> Steps { get; set; } = new();
}