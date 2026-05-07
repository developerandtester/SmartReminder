namespace SmartReminder.Application.DTOs.Pomodoro;

public class PomodoroStatsResponse
{
    public int TotalSessions { get; set; }

    public int CompletedSessions { get; set; }

    public int RunningSessions { get; set; }

    public int PausedSessions { get; set; }

    public int TotalFocusMinutes { get; set; }

    public int TodayFocusMinutes { get; set; }
}