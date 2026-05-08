namespace SmartReminder.Application.DTOs.VisualSchedules;

public class NowNextLaterResponse
{
    public VisualScheduleItemResponse? Now { get; set; }

    public VisualScheduleItemResponse? Next { get; set; }

    public List<VisualScheduleItemResponse> Later { get; set; } = new();
}