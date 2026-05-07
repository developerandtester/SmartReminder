using SmartReminder.Domain.Common;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Domain.Entities;

public class StudentParentLink : BaseEntity
{
    public int StudentId { get; set; }

    public int ParentId { get; set; }

    public LinkStatus Status { get; set; } = LinkStatus.Pending;

    public bool CanViewProgress { get; set; } = true;

    public bool CanCreateTasks { get; set; }

    public bool CanReceiveCriticalAlerts { get; set; }

    public bool CanViewVisualSchedule { get; set; } = true;

    public bool CanViewMoodNotes { get; set; }

    public AppUser Student { get; set; } = null!;

    public AppUser Parent { get; set; } = null!;
}