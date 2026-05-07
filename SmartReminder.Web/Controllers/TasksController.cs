using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartReminder.Application.DTOs.Tasks;
using SmartReminder.Application.Interfaces;

namespace SmartReminder.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _taskService.CreateTaskAsync(userId, request);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTasks()
    {
        var userId = GetCurrentUserId();
        var response = await _taskService.GetMyTasksAsync(userId);

        return Ok(response);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredTasks([FromQuery] TaskFilterRequest filter)
    {
        var userId = GetCurrentUserId();
        var response = await _taskService.GetFilteredTasksAsync(userId, filter);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _taskService.GetTaskByIdAsync(userId, id);

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _taskService.UpdateTaskAsync(userId, id, request);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> CompleteTask(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _taskService.CompleteTaskAsync(userId, id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPatch("{id:int}/snooze")]
    public async Task<IActionResult> SnoozeTask(int id, SnoozeTaskRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _taskService.SnoozeTaskAsync(userId, id, request);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPatch("{taskId:int}/steps/{stepId:int}/complete")]
    public async Task<IActionResult> CompleteTaskStep(int taskId, int stepId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _taskService.CompleteTaskStepAsync(userId, taskId, stepId);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _taskService.DeleteTaskAsync(userId, id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        return userId;
    }
}