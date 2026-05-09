using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CompanyDashboardAPI.Services;

public interface IAiService
{
    Task<string> GetChatResponseAsync(string userMessage, string systemPrompt = "");
}

public class GroqAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<GroqAiService> _logger;

    public GroqAiService(HttpClient httpClient, IConfiguration config, ILogger<GroqAiService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GetChatResponseAsync(string userMessage, string systemPrompt = "")
    {
        try
        {
            var apiKey = _config["AiConfig:ApiKey"];
            var model = _config["AiConfig:Model"] ?? "llama-3.3-70b-versatile";
            var apiUrl = _config["AiConfig:ApiUrl"] ?? "https://api.groq.com/openai/v1/chat/completions";

            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GROQ_API_KEY_HERE")
            {
                return "Groq API Key is not configured. Please add it to appsettings.json.";
            }

            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            
            // إضافة المفتاح بشكل مباشر في كل طلب لضمان الأمان والدقة
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Groq API error: {response.StatusCode} - {error}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return "المساعد مشغول حالياً، يرجى المحاولة بعد دقيقة.";
                }
                
                return $"خطأ من المزود ({response.StatusCode}): يرجى التأكد من الـ API Key.";
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GroqResponse>(responseBody);

            return result?.Choices?[0]?.Message?.Content ?? "لم أتمكن من الحصول على رد.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GroqAiService");
            return "عذراً، حدث خطأ غير متوقع.";
        }
    }
}

// DTOs for Groq (OpenAI Compatible)
public class GroqResponse
{
    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }
}

public class Choice
{
    [JsonPropertyName("message")]
    public Message? Message { get; set; }
}

public class Message
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
