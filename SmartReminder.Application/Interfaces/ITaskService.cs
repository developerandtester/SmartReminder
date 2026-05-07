using SmartReminder.Application.DTOs.Tasks;

namespace SmartReminder.Application.Interfaces;

public interface ITaskService
{
    Task<TaskResponse> CreateTaskAsync(int currentUserId, CreateTaskRequest request);

    Task<List<TaskResponse>> GetMyTasksAsync(int currentUserId);

    Task<TaskResponse> GetTaskByIdAsync(int currentUserId, int taskId);

    Task<TaskResponse> UpdateTaskAsync(int currentUserId, int taskId, UpdateTaskRequest request);

    Task CompleteTaskAsync(int currentUserId, int taskId);

    Task DeleteTaskAsync(int currentUserId, int taskId);
}