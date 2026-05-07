using SmartReminder.Domain.Common;

namespace SmartReminder.Domain.Entities;

public class ConversationParticipant : BaseEntity
{
    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public Conversation Conversation { get; set; } = null!;

    public AppUser User { get; set; } = null!;
}