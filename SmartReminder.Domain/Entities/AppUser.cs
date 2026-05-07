using SmartReminder.Domain.Common;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Domain.Entities;

public class AppUser : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ReminderTask> OwnedTasks { get; set; } = new List<ReminderTask>();

    public ICollection<ReminderTask> CreatedTasks { get; set; } = new List<ReminderTask>();

    public ICollection<PomodoroSession> PomodoroSessions { get; set; } = new List<PomodoroSession>();

    public ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
}