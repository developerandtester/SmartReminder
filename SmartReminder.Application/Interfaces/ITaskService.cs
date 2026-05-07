using SmartReminder.Application.DTOs.Tasks;

namespace SmartReminder.Application.Interfaces;

public interface ITaskService
{
    Task<TaskResponse> CreateTaskAsync(int currentUserId, CreateTaskRequest request);

    Task<List<TaskResponse>> GetMyTasksAsync(int currentUserId);

    Task<List<TaskResponse>> GetFilteredTasksAsync(int currentUserId, TaskFilterRequest filter);

    Task<TaskResponse> GetTaskByIdAsync(int currentUserId, int taskId);

    Task<TaskResponse> UpdateTaskAsync(int currentUserId, int taskId, UpdateTaskRequest request);

    Task CompleteTaskAsync(int currentUserId, int taskId);

    Task SnoozeTaskAsync(int currentUserId, int taskId, SnoozeTaskRequest request);

    Task CompleteTaskStepAsync(int currentUserId, int taskId, int stepId);

    Task DeleteTaskAsync(int currentUserId, int taskId);
}