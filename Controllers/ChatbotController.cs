using System.Text;
using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanyDashboardAPI.Controllers;

public class ChatRequestDto
{
    public string Message { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly ILocalAIService _aiService;

    public ChatbotController(ILocalAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { message = "Message cannot be empty." });
        }

        var systemPrompt = "You are Jobito, a helpful assistant for a job portal platform. Answer concisely and professionally.";
        
        var responseBuilder = new StringBuilder();
        await foreach (var token in _aiService.GetChatResponseAsync(systemPrompt, request.Message))
        {
            responseBuilder.Append(token);
        }

        return Ok(new { response = responseBuilder.ToString().Trim() });
    }
}
