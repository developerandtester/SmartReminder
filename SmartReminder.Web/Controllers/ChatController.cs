using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartReminder.Application.DTOs.Chat;
using SmartReminder.Application.Interfaces;
using SmartReminder.Web.Hubs;

namespace SmartReminder.Web.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _chatHubContext;

    public ChatController(
        IChatService chatService,
        IHubContext<ChatHub> chatHubContext)
    {
        _chatService = chatService;
        _chatHubContext = chatHubContext;
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation(StartConversationRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _chatService.StartConversationAsync(userId, request);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations()
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _chatService.GetMyConversationsAsync(userId);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpGet("conversations/{conversationId:int}/messages")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _chatService.GetMessagesAsync(userId, conversationId);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return MapException(ex);
        }
    }

    [HttpPost("conversations/{conversationId:int}/messages")]
    public async Task<IActionResult> SendMessage(int conversationId, SendMessageRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var response = await _chatService.SendMessageAsync(userId, conversationId, request);

            await _chatHubContext
                .Clients
                .Group(ChatHub.GetGroupName(conversationId.ToString()))
                .SendAsync("ReceiveMessage", response);

            return Ok(response);
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