using SmartReminder.Domain.Common;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Domain.Entities;

public class StudentTeacherLink : BaseEntity
{
    public int StudentId { get; set; }

    public int TeacherId { get; set; }

    public LinkStatus Status { get; set; } = LinkStatus.Pending;

    public bool CanViewProgress { get; set; } = true;

    public bool CanCreateTasks { get; set; } = true;

    public bool CanViewVisualSchedule { get; set; } = true;

    public AppUser Student { get; set; } = null!;

    public AppUser Teacher { get; set; } = null!;
}