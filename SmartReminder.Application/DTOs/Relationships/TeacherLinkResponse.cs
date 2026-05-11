using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.DTOs.Relationships;

public class TeacherLinkResponse
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string StudentEmail { get; set; } = string.Empty;

    public int TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public string TeacherEmail { get; set; } = string.Empty;

    public LinkStatus Status { get; set; }

    public bool CanViewProgress { get; set; }

    public bool CanCreateTasks { get; set; }

    public bool CanViewVisualSchedule { get; set; }
}