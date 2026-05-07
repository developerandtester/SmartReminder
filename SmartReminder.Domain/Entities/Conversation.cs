using SmartReminder.Domain.Common;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Domain.Entities;

public class Conversation : BaseEntity
{
    public ConversationType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}