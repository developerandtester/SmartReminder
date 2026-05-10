using SmartReminder.Application.DTOs.Pomodoro;
using SmartReminder.Application.Interfaces;
using SmartReminder.Domain.Entities;
using SmartReminder.Domain.Enums;

namespace SmartReminder.Application.Services;

public class PomodoroService : IPomodoroService
{
    private readonly IPomodoroRepository _pomodoroRepository;
    private readonly ITaskRepository _taskRepository;

    public PomodoroService(
        IPomodoroRepository pomodoroRepository,
        ITaskRepository taskRepository)
    {
        _pomodoroRepository = pomodoroRepository;
        _taskRepository = taskRepository;
    }

    public async Task<PomodoroResponse> StartAsync(int currentUserId, StartPomodoroRequest request)
    {
        ValidateStartRequest(request);

        if (request.ReminderTaskId.HasValue)
        {
            var task = await _taskRepository.GetByIdAsync(request.ReminderTaskId.Value);

            if (task == null)
            {
                throw new KeyNotFoundException("Linked task not found.");
            }

            if (task.OwnerUserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You cannot start Pomodoro for this task.");
            }

            if (task.Status == ReminderStatus.Completed ||
                task.Status == ReminderStatus.Cancelled ||
                task.Status == ReminderStatus.Rejected)
            {
                throw new InvalidOperationException("Cannot start Pomodoro for a closed task.");
            }
        }

        var session = new PomodoroSession
        {
            UserId = currentUserId,
            ReminderTaskId = request.ReminderTaskId,
            FocusDurationMinutes = request.FocusDurationMinutes,
            ShortBreakMinutes = request.ShortBreakMinutes,
            LongBreakMinutes = request.LongBreakMinutes,
            CycleNumber = request.CycleNumber,
            Status = PomodoroStatus.Running,
            StartedAtUtc = DateTime.UtcNow
        };

        await _pomodoroRepository.AddAsync(session);
        await _pomodoroRepository.SaveChangesAsync();

        var createdSession = await _pomodoroRepository.GetByIdAsync(session.Id);

        return MapToResponse(createdSession ?? session);
    }

    public async Task PauseAsync(int currentUserId, int sessionId)
    {
        var session = await GetOwnedSessionOrThrowAsync(currentUserId, sessionId);

        if (session.Status != PomodoroStatus.Running)
        {
            throw new InvalidOperationException("Only a running Pomodoro session can be paused.");
        }

        session.Status = PomodoroStatus.Paused;
        session.PausedAtUtc = DateTime.UtcNow;
        session.UpdatedAtUtc = DateTime.UtcNow;

        await _pomodoroRepository.SaveChangesAsync();
    }

    public async Task ResumeAsync(int currentUserId, int sessionId)
    {
        var session = await GetOwnedSessionOrThrowAsync(currentUserId, sessionId);

        if (session.Status != PomodoroStatus.Paused)
        {
            throw new InvalidOperationException("Only a paused Pomodoro session can be resumed.");
        }

        session.Status = PomodoroStatus.Running;
        session.PausedAtUtc = null;
        session.UpdatedAtUtc = DateTime.UtcNow;

        await _pomodoroRepository.SaveChangesAsync();
    }

    public async Task CompleteAsync(int currentUserId, int sessionId, CompletePomodoroRequest request)
    {
        var session = await GetOwnedSessionOrThrowAsync(currentUserId, sessionId);

        if (session.Status == PomodoroStatus.Completed)
        {
            throw new InvalidOperationException("Pomodoro session is already completed.");
        }

        if (session.Status == PomodoroStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled Pomodoro session cannot be completed.");
        }

        var completedAtUtc = DateTime.UtcNow;

        session.Status = PomodoroStatus.Completed;
        session.CompletedAtUtc = completedAtUtc;
        session.Notes = request.Notes?.Trim();
        session.UpdatedAtUtc = completedAtUtc;

        if (session.StartedAtUtc.HasValue)
        {
            var actualMinutes = (int)Math.Ceiling((completedAtUtc - session.StartedAtUtc.Value).TotalMinutes);

            session.ActualFocusMinutes = Math.Min(
                Math.Max(actualMinutes, 1),
                session.FocusDurationMinutes
            );
        }
        else
        {
            session.ActualFocusMinutes = 0;
        }

        if (request.MarkLinkedTaskCompleted && session.ReminderTask != null)
        {
            if (session.ReminderTask.OwnerUserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You cannot complete this linked task.");
            }

            session.ReminderTask.Status = ReminderStatus.Completed;
            session.ReminderTask.UpdatedAtUtc = DateTime.UtcNow;

            foreach (var step in session.ReminderTask.Steps)
            {
                step.IsCompleted = true;
                step.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        await _pomodoroRepository.SaveChangesAsync();
    }

    public async Task<List<PomodoroResponse>> GetMySessionsAsync(int currentUserId)
    {
        var sessions = await _pomodoroRepository.GetSessionsForUserAsync(currentUserId);

        return sessions
            .OrderByDescending(x => x.StartedAtUtc)
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<PomodoroStatsResponse> GetMyStatsAsync(int currentUserId)
    {
        var sessions = await _pomodoroRepository.GetSessionsForUserAsync(currentUserId);

        var today = DateTime.UtcNow.Date;

        var completedSessions = sessions
            .Where(x => x.Status == PomodoroStatus.Completed)
            .ToList();

        return new PomodoroStatsResponse
        {
            TotalSessions = sessions.Count,
            CompletedSessions = completedSessions.Count,
            RunningSessions = sessions.Count(x => x.Status == PomodoroStatus.Running),
            PausedSessions = sessions.Count(x => x.Status == PomodoroStatus.Paused),
            TotalFocusMinutes = completedSessions.Sum(x => x.ActualFocusMinutes),
            TodayFocusMinutes = completedSessions
    .Where(x => x.CompletedAtUtc.HasValue && x.CompletedAtUtc.Value.Date == today)
    .Sum(x => x.ActualFocusMinutes)
        };
    }

    private async Task<PomodoroSession> GetOwnedSessionOrThrowAsync(int currentUserId, int sessionId)
    {
        var session = await _pomodoroRepository.GetByIdAsync(sessionId);

        if (session == null)
        {
            throw new KeyNotFoundException("Pomodoro session not found.");
        }

        if (session.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You are not allowed to access this Pomodoro session.");
        }

        return session;
    }

    private static void ValidateStartRequest(StartPomodoroRequest request)
    {
        if (request.FocusDurationMinutes <= 0 || request.FocusDurationMinutes > 180)
        {
            throw new ArgumentException("Focus duration must be between 1 and 180 minutes.");
        }

        if (request.ShortBreakMinutes < 0 || request.ShortBreakMinutes > 60)
        {
            throw new ArgumentException("Short break must be between 0 and 60 minutes.");
        }

        if (request.LongBreakMinutes < 0 || request.LongBreakMinutes > 120)
        {
            throw new ArgumentException("Long break must be between 0 and 120 minutes.");
        }

        if (request.CycleNumber <= 0 || request.CycleNumber > 20)
        {
            throw new ArgumentException("Cycle number must be between 1 and 20.");
        }
    }

    private static PomodoroResponse MapToResponse(PomodoroSession session)
    {
        return new PomodoroResponse
        {
            Id = session.Id,
            UserId = session.UserId,
            ReminderTaskId = session.ReminderTaskId,
            LinkedTaskTitle = session.ReminderTask?.Title,
            ActualFocusMinutes = session.ActualFocusMinutes,
            FocusDurationMinutes = session.FocusDurationMinutes,
            ShortBreakMinutes = session.ShortBreakMinutes,
            LongBreakMinutes = session.LongBreakMinutes,
            CycleNumber = session.CycleNumber,
            Status = session.Status,
            StartedAtUtc = session.StartedAtUtc,
            PausedAtUtc = session.PausedAtUtc,
            CompletedAtUtc = session.CompletedAtUtc,
            Notes = session.Notes
        };
    }
}