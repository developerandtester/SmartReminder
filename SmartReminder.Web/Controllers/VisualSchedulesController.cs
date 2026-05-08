using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartReminder.Application.DTOs.VisualSchedules;
using SmartReminder.Application.Interfaces;

namespace SmartReminder.Web.Controllers;

[ApiController]
[Route("api/visual-schedules")]
[Authorize]
public class VisualSchedulesController : ControllerBase
{
    private readonly IVisualScheduleService _visualScheduleService;

    public VisualSchedulesController(IVisualScheduleService visualScheduleService)
    {
        _visualScheduleService = visualScheduleService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVisualScheduleRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _visualScheduleService.CreateAsync(userId, request);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
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

    [HttpGet("my/today")]
    public async Task<IActionResult> GetToday()
    {
        var userId = GetCurrentUserId();
        var response = await _visualScheduleService.GetTodayAsync(userId);

        if (response == null)
        {
            return NotFound(new { message = "No visual schedule found for today." });
        }

        return Ok(response);
    }

    [HttpGet("my/date/{date}")]
    public async Task<IActionResult> GetByDate(DateOnly date)
    {
        var userId = GetCurrentUserId();
        var response = await _visualScheduleService.GetByDateAsync(userId, date);

        if (response == null)
        {
            return NotFound(new { message = "No visual schedule found for this date." });
        }

        return Ok(response);
    }

    [HttpGet("my/now-next-later")]
    public async Task<IActionResult> GetNowNextLater()
    {
        var userId = GetCurrentUserId();
        var response = await _visualScheduleService.GetNowNextLaterAsync(userId);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _visualScheduleService.GetByIdAsync(userId, id);

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

    [HttpPatch("{scheduleId:int}/items/{itemId:int}/link-task/{taskId:int}")]
    public async Task<IActionResult> LinkItemToTask(int scheduleId, int itemId, int taskId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _visualScheduleService.LinkItemToTaskAsync(userId, scheduleId, itemId, taskId);

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
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _visualScheduleService.DeleteAsync(userId, id);

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