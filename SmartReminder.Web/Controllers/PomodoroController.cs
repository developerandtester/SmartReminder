using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartReminder.Application.DTOs.Pomodoro;
using SmartReminder.Application.Interfaces;

namespace SmartReminder.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PomodoroController : ControllerBase
{
    private readonly IPomodoroService _pomodoroService;

    public PomodoroController(IPomodoroService pomodoroService)
    {
        _pomodoroService = pomodoroService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(StartPomodoroRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _pomodoroService.StartAsync(userId, request);

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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/pause")]
    public async Task<IActionResult> Pause(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _pomodoroService.PauseAsync(userId, id);

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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/resume")]
    public async Task<IActionResult> Resume(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _pomodoroService.ResumeAsync(userId, id);

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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id, CompletePomodoroRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _pomodoroService.CompleteAsync(userId, id, request);

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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMySessions()
    {
        var userId = GetCurrentUserId();
        var response = await _pomodoroService.GetMySessionsAsync(userId);

        return Ok(response);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userId = GetCurrentUserId();
        var response = await _pomodoroService.GetMyStatsAsync(userId);

        return Ok(response);
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