using SmartReminder.Application.DTOs.Pomodoro;

namespace SmartReminder.Application.Interfaces;

public interface IPomodoroService
{
    Task<PomodoroResponse> StartAsync(int currentUserId, StartPomodoroRequest request);

    Task PauseAsync(int currentUserId, int sessionId);

    Task ResumeAsync(int currentUserId, int sessionId);

    Task CompleteAsync(int currentUserId, int sessionId, CompletePomodoroRequest request);

    Task<List<PomodoroResponse>> GetMySessionsAsync(int currentUserId);

    Task<PomodoroStatsResponse> GetMyStatsAsync(int currentUserId);
}