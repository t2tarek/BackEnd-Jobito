using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanyDashboardAPI.Controllers;

public class TranslateRequestDto
{
    public string Text { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = "ar";
    public string SourceLanguage { get; set; } = "auto";
}

[ApiController]
[Route("api/[controller]")]
public class TranslationController : ControllerBase
{
    private readonly ITranslationService _translationService;

    public TranslationController(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    [HttpPost]
    public async Task<IActionResult> Translate([FromBody] TranslateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new { message = "Text cannot be empty." });
        }

        var translatedText = await _translationService.TranslateTextAsync(request.Text, request.TargetLanguage, request.SourceLanguage);
        
        return Ok(new
        {
            originalText = request.Text,
            translatedText = translatedText,
            targetLanguage = request.TargetLanguage
        });
    }
}
