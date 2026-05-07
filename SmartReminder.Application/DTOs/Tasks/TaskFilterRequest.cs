using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.DTOs.Tasks;

public class TaskFilterRequest
{
    public ReminderStatus? Status { get; set; }

    public ReminderPriority? Priority { get; set; }

    public DateTime? DueFromUtc { get; set; }

    public DateTime? DueToUtc { get; set; }
}