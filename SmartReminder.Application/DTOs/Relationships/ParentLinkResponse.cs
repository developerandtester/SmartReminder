using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.DTOs.Relationships;

public class ParentLinkResponse
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string StudentEmail { get; set; } = string.Empty;

    public int ParentId { get; set; }

    public string ParentName { get; set; } = string.Empty;

    public string ParentEmail { get; set; } = string.Empty;

    public LinkStatus Status { get; set; }

    public bool CanViewProgress { get; set; }

    public bool CanCreateTasks { get; set; }

    public bool CanReceiveCriticalAlerts { get; set; }

    public bool CanViewVisualSchedule { get; set; }

    public bool CanViewMoodNotes { get; set; }
}