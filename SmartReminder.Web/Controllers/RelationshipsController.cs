using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartReminder.Application.DTOs.Relationships;
using SmartReminder.Application.Interfaces;

namespace SmartReminder.Web.Controllers;

[ApiController]
[Route("api/relationships")]
[Authorize]
public class RelationshipsController : ControllerBase
{
    private readonly IRelationshipService _relationshipService;

    public RelationshipsController(IRelationshipService relationshipService)
    {
        _relationshipService = relationshipService;
    }

    [HttpPost("parent/invite-student")]
    public async Task<IActionResult> ParentInviteStudent(InviteStudentRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _relationshipService.InviteStudentAsParentAsync(userId, request);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpPost("teacher/invite-student")]
    public async Task<IActionResult> TeacherInviteStudent(InviteStudentRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _relationshipService.InviteStudentAsTeacherAsync(userId, request);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpGet("student/pending")]
    public async Task<IActionResult> GetPendingForStudent()
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _relationshipService.GetPendingInvitesForStudentAsync(userId);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpGet("parent/my-students")]
    public async Task<IActionResult> GetMyStudentsAsParent()
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _relationshipService.GetMyStudentsAsParentAsync(userId);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpGet("teacher/my-students")]
    public async Task<IActionResult> GetMyStudentsAsTeacher()
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _relationshipService.GetMyStudentsAsTeacherAsync(userId);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpPatch("student/parent-links/{id:int}/accept")]
    public async Task<IActionResult> AcceptParentLink(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _relationshipService.AcceptParentLinkAsync(userId, id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpPatch("student/parent-links/{id:int}/reject")]
    public async Task<IActionResult> RejectParentLink(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _relationshipService.RejectParentLinkAsync(userId, id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpPatch("student/teacher-links/{id:int}/accept")]
    public async Task<IActionResult> AcceptTeacherLink(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _relationshipService.AcceptTeacherLinkAsync(userId, id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpPatch("student/teacher-links/{id:int}/reject")]
    public async Task<IActionResult> RejectTeacherLink(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _relationshipService.RejectTeacherLinkAsync(userId, id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return MapException(ex);
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

    private IActionResult MapException(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => NotFound(new { message = ex.Message }),
            UnauthorizedAccessException => Forbid(),
            InvalidOperationException => BadRequest(new { message = ex.Message }),
            ArgumentException => BadRequest(new { message = ex.Message }),
            _ => BadRequest(new { message = ex.Message })
        };
    }
}