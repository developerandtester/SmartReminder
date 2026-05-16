using SmartReminder.Application.DTOs.Tasks;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRelationshipRepository _relationshipRepository;

    public TaskService(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    IRelationshipRepository relationshipRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _relationshipRepository = relationshipRepository;
    }



    public async Task<TaskResponse> CreateTaskAsync(int currentUserId, CreateTaskRequest request)
    {
        ValidateCreateRequest(request);

        var task = new ReminderTask
        {
            OwnerUserId = currentUserId,
            CreatedByUserId = currentUserId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DueAtUtc = request.DueAtUtc,
            Priority = request.Priority,
            Status = ReminderStatus.Pending,
            IsSuggestedByParentOrTeacher = false,
            RequiresStudentApproval = false,
            Steps = request.Steps
                .Where(step => !string.IsNullOrWhiteSpace(step))
                .Select((step, index) => new TaskStep
                {
                    Title = step.Trim(),
                    SortOrder = index + 1
                })
                .ToList()
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return MapToResponse(task);
    }

    public async Task<List<TaskResponse>> GetMyTasksAsync(int currentUserId)
    {
        var tasks = await _taskRepository.GetTasksForUserAsync(currentUserId);

        return tasks
            .Select(MapToResponse)
            .ToList();
    }



    public async Task<List<TaskResponse>> GetFilteredTasksAsync(int currentUserId, TaskFilterRequest filter)
    {
        var tasks = await _taskRepository.GetFilteredTasksForUserAsync(currentUserId, filter);

        return tasks
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<TaskResponse> GetTaskByIdAsync(int currentUserId, int taskId)
    {
        var task = await GetOwnedTaskOrThrowAsync(currentUserId, taskId);

        return MapToResponse(task);
    }

    public async Task<TaskResponse> UpdateTaskAsync(int currentUserId, int taskId, UpdateTaskRequest request)
    {
        ValidateUpdateRequest(request);

        var task = await GetOwnedTaskOrThrowAsync(currentUserId, taskId);

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.DueAtUtc = request.DueAtUtc;
        task.Priority = request.Priority;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _taskRepository.SaveChangesAsync();

        return MapToResponse(task);
    }

    public async Task CompleteTaskAsync(int currentUserId, int taskId)
    {
        var task = await GetOwnedTaskOrThrowAsync(currentUserId, taskId);

        task.Status = ReminderStatus.Completed;
        task.UpdatedAtUtc = DateTime.UtcNow;

        foreach (var step in task.Steps)
        {
            step.IsCompleted = true;
            step.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _taskRepository.SaveChangesAsync();
    }

    public async Task SnoozeTaskAsync(int currentUserId, int taskId, SnoozeTaskRequest request)
    {
        if (request.NewDueAtUtc <= DateTime.UtcNow)
        {
            throw new ArgumentException("Snooze time must be in the future.");
        }

        var task = await GetOwnedTaskOrThrowAsync(currentUserId, taskId);

        task.DueAtUtc = request.NewDueAtUtc;
        task.Status = ReminderStatus.Snoozed;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _taskRepository.SaveChangesAsync();
    }

    public async Task CompleteTaskStepAsync(int currentUserId, int taskId, int stepId)
    {
        var task = await GetOwnedTaskOrThrowAsync(currentUserId, taskId);

        var step = task.Steps.FirstOrDefault(x => x.Id == stepId);

        if (step == null)
        {
            throw new KeyNotFoundException("Task step not found.");
        }

        step.IsCompleted = true;
        step.UpdatedAtUtc = DateTime.UtcNow;

        var allStepsCompleted = task.Steps.Any() && task.Steps.All(x => x.IsCompleted);

        if (allStepsCompleted)
        {
            task.Status = ReminderStatus.Completed;
            task.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _taskRepository.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int currentUserId, int taskId)
    {
        var task = await GetOwnedTaskOrThrowAsync(currentUserId, taskId);

        _taskRepository.Remove(task);

        await _taskRepository.SaveChangesAsync();
    }

    private async Task<ReminderTask> GetOwnedTaskOrThrowAsync(int currentUserId, int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        if (task.OwnerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You are not allowed to access this task.");
        }

        return task;
    }

    private static void ValidateCreateRequest(CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.");
        }

        if (!Enum.IsDefined(typeof(ReminderPriority), request.Priority))
        {
            throw new ArgumentException("Invalid priority.");
        }
    }

    private static void ValidateUpdateRequest(UpdateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.");
        }

        if (!Enum.IsDefined(typeof(ReminderPriority), request.Priority))
        {
            throw new ArgumentException("Invalid priority.");
        }
    }

    public async Task<TaskResponse> AssignTaskToStudentAsync(
    int currentUserId,
    int studentId,
    AssignTaskToStudentRequest request)
    {
        ValidateAssignRequest(request);

        var assigner = await _userRepository.GetByIdAsync(currentUserId);

        if (assigner == null)
        {
            throw new KeyNotFoundException("Current user not found.");
        }

        var student = await _userRepository.GetByIdAsync(studentId);

        if (student == null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        if (student.Role != UserRole.Student)
        {
            throw new InvalidOperationException("Target user is not a student.");
        }

        var canAssign = await CanAssignTaskToStudentAsync(assigner.Id, student.Id, assigner.Role);

        if (!canAssign)
        {
            throw new UnauthorizedAccessException("You are not allowed to assign tasks to this student.");
        }

        var task = new ReminderTask
        {
            OwnerUserId = student.Id,
            CreatedByUserId = assigner.Id,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DueAtUtc = request.DueAtUtc,
            Priority = request.Priority,
            Status = ReminderStatus.Pending,
            IsSuggestedByParentOrTeacher = true,
            RequiresStudentApproval = false,
            Steps = request.Steps
                .Where(step => !string.IsNullOrWhiteSpace(step))
                .Select((step, index) => new TaskStep
                {
                    Title = step.Trim(),
                    SortOrder = index + 1
                })
                .ToList()
        };

        if (!task.Steps.Any())
        {
            throw new ArgumentException("Assigned tasks must have at least one guided step.");
        }

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return MapToResponse(task);
    }
    private async Task<bool> CanAssignTaskToStudentAsync(
    int assignerId,
    int studentId,
    UserRole assignerRole)
    {
        if (assignerRole == UserRole.Admin)
        {
            return true;
        }

        if (assignerRole == UserRole.Parent)
        {
            var parentLink = await _relationshipRepository.GetParentLinkAsync(studentId, assignerId);

            return parentLink != null &&
                   parentLink.Status == LinkStatus.Accepted &&
                   parentLink.CanCreateTasks;
        }

        if (assignerRole == UserRole.Teacher)
        {
            var teacherLink = await _relationshipRepository.GetTeacherLinkAsync(studentId, assignerId);

            return teacherLink != null &&
                   teacherLink.Status == LinkStatus.Accepted &&
                   teacherLink.CanCreateTasks;
        }

        return false;
    }

    private static void ValidateAssignRequest(AssignTaskToStudentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.");
        }

        if (!Enum.IsDefined(typeof(ReminderPriority), request.Priority))
        {
            throw new ArgumentException("Invalid priority.");
        }

        if (request.Steps == null || request.Steps.All(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Assigned guided task must include at least one step.");
        }
    }

    private static TaskResponse MapToResponse(ReminderTask task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            OwnerUserId = task.OwnerUserId,
            CreatedByUserId = task.CreatedByUserId,
            Title = task.Title,
            Description = task.Description,
            DueAtUtc = task.DueAtUtc,
            Priority = task.Priority,
            Status = task.Status,
            MissedCount = task.MissedCount,
            IsSuggestedByParentOrTeacher = task.IsSuggestedByParentOrTeacher,
            RequiresStudentApproval = task.RequiresStudentApproval,
            Steps = task.Steps
                .OrderBy(step => step.SortOrder)
                .Select(step => new TaskStepResponse
                {
                    Id = step.Id,
                    Title = step.Title,
                    IsCompleted = step.IsCompleted,
                    SortOrder = step.SortOrder
                })
                .ToList()
        };
    }
}