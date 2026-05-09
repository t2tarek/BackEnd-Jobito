using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CompanyDashboardAPI.Services;

public interface IGeminiService
{
    Task<string> GetChatResponseAsync(string userMessage, string systemPrompt = "");
}

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(HttpClient httpClient, IConfiguration config, ILogger<GeminiService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GetChatResponseAsync(string userMessage, string systemPrompt = "")
    {
        try
        {
            var apiKey = _config["Gemini:ApiKey"];
            var model = _config["Gemini:Model"] ?? "gemini-1.5-flash";
            
            // رابط ثابت ومضمون لتجنب أي مشاكل في الإعدادات
            var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = string.IsNullOrEmpty(systemPrompt) ? userMessage : $"{systemPrompt}\n\nUser: {userMessage}" }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Gemini API error: {response.StatusCode} - {error}");
                return $"خطأ من جوجل ({response.StatusCode}): يرجى التأكد من الـ API Key.";
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GeminiResponse>(responseBody);

            return result?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "لم أتمكن من الحصول على رد.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GeminiService");
            return "عذراً، حدث خطأ غير متوقع.";
        }
    }
}

// DTOs for Gemini Response
public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<Candidate>? Candidates { get; set; }
}

public class Candidate
{
    [JsonPropertyName("content")]
    public Content? Content { get; set; }
}

public class Content
{
    [JsonPropertyName("parts")]
    public List<Part>? Parts { get; set; }
}

public class Part
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
