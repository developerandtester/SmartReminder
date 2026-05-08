using SmartReminder.Application.DTOs.VisualSchedules;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;

namespace SmartReminder.Application.Services;

public class VisualScheduleService : IVisualScheduleService
{
    private readonly IVisualScheduleRepository _visualScheduleRepository;
    private readonly ITaskRepository _taskRepository;

    public VisualScheduleService(
        IVisualScheduleRepository visualScheduleRepository,
        ITaskRepository taskRepository)
    {
        _visualScheduleRepository = visualScheduleRepository;
        _taskRepository = taskRepository;
    }

    public async Task<VisualScheduleResponse> CreateAsync(int currentUserId, CreateVisualScheduleRequest request)
    {
        ValidateCreateRequest(request);

        var existingSchedule = await _visualScheduleRepository
            .GetForStudentByDateAsync(currentUserId, request.ScheduleDate);

        if (existingSchedule != null)
        {
            throw new InvalidOperationException("A visual schedule already exists for this date.");
        }

        foreach (var item in request.Items.Where(x => x.LinkedTaskId.HasValue))
        {
            var task = await _taskRepository.GetByIdAsync(item.LinkedTaskId!.Value);

            if (task == null)
            {
                throw new KeyNotFoundException($"Linked task {item.LinkedTaskId.Value} was not found.");
            }

            if (task.OwnerUserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You cannot link a task that does not belong to you.");
            }
        }

        var schedule = new VisualSchedule
        {
            StudentId = currentUserId,
            CreatedByUserId = currentUserId,
            ScheduleDate = request.ScheduleDate,
            Title = request.Title.Trim(),
            Items = request.Items
                .OrderBy(x => x.SortOrder)
                .Select((item, index) => new VisualScheduleItem
                {
                    LinkedTaskId = item.LinkedTaskId,
                    Title = item.Title.Trim(),
                    Description = item.Description?.Trim(),
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    IconName = item.IconName?.Trim(),
                    ColorCode = item.ColorCode?.Trim(),
                    ItemType = item.ItemType,
                    SortOrder = item.SortOrder == 0 ? index + 1 : item.SortOrder
                })
                .ToList()
        };

        await _visualScheduleRepository.AddAsync(schedule);
        await _visualScheduleRepository.SaveChangesAsync();

        var createdSchedule = await _visualScheduleRepository.GetByIdAsync(schedule.Id);

        return MapToResponse(createdSchedule ?? schedule);
    }

    public async Task<VisualScheduleResponse?> GetTodayAsync(int currentUserId)
    {
        return await GetByDateAsync(currentUserId, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    public async Task<VisualScheduleResponse?> GetByDateAsync(int currentUserId, DateOnly date)
    {
        var schedule = await _visualScheduleRepository.GetForStudentByDateAsync(currentUserId, date);

        return schedule == null ? null : MapToResponse(schedule);
    }

    public async Task<VisualScheduleResponse> GetByIdAsync(int currentUserId, int scheduleId)
    {
        var schedule = await GetOwnedScheduleOrThrowAsync(currentUserId, scheduleId);

        return MapToResponse(schedule);
    }

    public async Task<NowNextLaterResponse> GetNowNextLaterAsync(int currentUserId)
    {
        var schedule = await _visualScheduleRepository
            .GetForStudentByDateAsync(currentUserId, DateOnly.FromDateTime(DateTime.UtcNow));

        if (schedule == null)
        {
            return new NowNextLaterResponse();
        }

        var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

        var orderedItems = schedule.Items
            .OrderBy(x => x.StartTime)
            .ThenBy(x => x.SortOrder)
            .ToList();

        var now = orderedItems
            .FirstOrDefault(x => x.StartTime <= currentTime && x.EndTime >= currentTime);

        var next = orderedItems
            .FirstOrDefault(x => x.StartTime > currentTime);

        var later = orderedItems
            .Where(x => x.StartTime > currentTime)
            .Skip(next == null ? 0 : 1)
            .Select(MapItemToResponse)
            .ToList();

        return new NowNextLaterResponse
        {
            Now = now == null ? null : MapItemToResponse(now),
            Next = next == null ? null : MapItemToResponse(next),
            Later = later
        };
    }

    public async Task LinkItemToTaskAsync(int currentUserId, int scheduleId, int itemId, int taskId)
    {
        var schedule = await GetOwnedScheduleOrThrowAsync(currentUserId, scheduleId);

        var item = schedule.Items.FirstOrDefault(x => x.Id == itemId);

        if (item == null)
        {
            throw new KeyNotFoundException("Visual schedule item not found.");
        }

        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        if (task.OwnerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You cannot link a task that does not belong to you.");
        }

        item.LinkedTaskId = taskId;
        item.UpdatedAtUtc = DateTime.UtcNow;

        await _visualScheduleRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int currentUserId, int scheduleId)
    {
        var schedule = await GetOwnedScheduleOrThrowAsync(currentUserId, scheduleId);

        _visualScheduleRepository.Remove(schedule);

        await _visualScheduleRepository.SaveChangesAsync();
    }

    private async Task<VisualSchedule> GetOwnedScheduleOrThrowAsync(int currentUserId, int scheduleId)
    {
        var schedule = await _visualScheduleRepository.GetByIdAsync(scheduleId);

        if (schedule == null)
        {
            throw new KeyNotFoundException("Visual schedule not found.");
        }

        if (schedule.StudentId != currentUserId)
        {
            throw new UnauthorizedAccessException("You are not allowed to access this visual schedule.");
        }

        return schedule;
    }

    private static void ValidateCreateRequest(CreateVisualScheduleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Schedule title is required.");
        }

        if (request.Items.Count == 0)
        {
            throw new ArgumentException("At least one schedule item is required.");
        }

        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                throw new ArgumentException("Schedule item title is required.");
            }

            if (item.EndTime <= item.StartTime)
            {
                throw new ArgumentException($"End time must be after start time for item: {item.Title}");
            }
        }
    }

    private static VisualScheduleResponse MapToResponse(VisualSchedule schedule)
    {
        return new VisualScheduleResponse
        {
            Id = schedule.Id,
            StudentId = schedule.StudentId,
            CreatedByUserId = schedule.CreatedByUserId,
            ScheduleDate = schedule.ScheduleDate,
            Title = schedule.Title,
            Items = schedule.Items
                .OrderBy(x => x.StartTime)
                .ThenBy(x => x.SortOrder)
                .Select(MapItemToResponse)
                .ToList()
        };
    }

    private static VisualScheduleItemResponse MapItemToResponse(VisualScheduleItem item)
    {
        return new VisualScheduleItemResponse
        {
            Id = item.Id,
            LinkedTaskId = item.LinkedTaskId,
            LinkedTaskTitle = item.LinkedTask?.Title,
            Title = item.Title,
            Description = item.Description,
            StartTime = item.StartTime,
            EndTime = item.EndTime,
            IconName = item.IconName,
            ColorCode = item.ColorCode,
            ItemType = item.ItemType,
            SortOrder = item.SortOrder
        };
    }
}