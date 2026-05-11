namespace SmartReminder.Application.DTOs.Chat;

public class ConversationResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public List<int> ParticipantUserIds { get; set; } = new();

    public string LastMessage { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}