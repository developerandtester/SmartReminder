using SmartReminder.Application.DTOs.VisualSchedules;

namespace SmartReminder.Application.Interfaces;

public interface IVisualScheduleService
{
    Task<VisualScheduleResponse> CreateAsync(int currentUserId, CreateVisualScheduleRequest request);

    Task<VisualScheduleResponse?> GetTodayAsync(int currentUserId);

    Task<VisualScheduleResponse?> GetByDateAsync(int currentUserId, DateOnly date);

    Task<VisualScheduleResponse> GetByIdAsync(int currentUserId, int scheduleId);

    Task<NowNextLaterResponse> GetNowNextLaterAsync(int currentUserId);

    Task LinkItemToTaskAsync(int currentUserId, int scheduleId, int itemId, int taskId);

    Task DeleteAsync(int currentUserId, int scheduleId);
}