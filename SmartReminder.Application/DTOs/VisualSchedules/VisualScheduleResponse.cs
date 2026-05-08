namespace SmartReminder.Application.DTOs.VisualSchedules;

public class VisualScheduleResponse
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CreatedByUserId { get; set; }

    public DateOnly ScheduleDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<VisualScheduleItemResponse> Items { get; set; } = new();
}