namespace SmartReminder.Application.DTOs.VisualSchedules;

public class CreateVisualScheduleRequest
{
    public DateOnly ScheduleDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<CreateVisualScheduleItemRequest> Items { get; set; } = new();
}