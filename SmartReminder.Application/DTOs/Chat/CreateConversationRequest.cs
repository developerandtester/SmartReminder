namespace SmartReminder.Application.DTOs.Chat;

public class CreateConversationRequest
{
    public int ParticipantUserId { get; set; }

    public string Title { get; set; } = string.Empty;
}