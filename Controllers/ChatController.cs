using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanyDashboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IAiService aiService, ILogger<ChatController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        if (string.IsNullOrEmpty(request.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        // Professional System Prompt for Jobito
        string systemPrompt = @"أنت مساعد ذكي لمنصة 'Jobito' (جوبيتو). 
        'Jobito' هي منصة توظيف متطورة تربط بين الباحثين عن عمل والشركات.
        مهامك:
        1. مساعدة المستخدمين في العثور على وظائف.
        2. الإجابة على استفسارات حول كيفية استخدام المنصة.
        3. تقديم نصائح مهنية.
        4. كن ودوداً، مهنياً، ودائماً تحدث باللغة التي يفضلها المستخدم.
        إذا سُئلت عن شيء خارج نطاق التوظيف، حاول إعادة توجيه المستخدم بلطف.";

        var response = await _aiService.GetChatResponseAsync(request.Message, systemPrompt);

        return Ok(new { response });
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}
