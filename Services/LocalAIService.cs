using LLama.Common;
using LLama;

namespace CompanyDashboardAPI.Services;

public interface ILocalAIService
{
    IAsyncEnumerable<string> GetChatResponseAsync(string systemPrompt, string userMessage);
}

public class LocalAIService : ILocalAIService, IDisposable
{
    private readonly LLamaWeights? _model;
    private readonly LLamaContext? _context;
    private readonly InteractiveExecutor? _executor;
    private readonly ILogger<LocalAIService> _logger;
    private readonly bool _isModelLoaded;

    public LocalAIService(IConfiguration config, ILogger<LocalAIService> logger)
    {
        _logger = logger;
        
        var modelPath = config["AI:ModelPath"] ?? "Models/qwen.gguf";

        if (File.Exists(modelPath))
        {
            try
            {
                var parameters = new ModelParams(modelPath)
                {
                    ContextSize = 1024,
                    GpuLayerCount = 5 // Use CPU mostly, but try to offload if possible
                };

                _model = LLamaWeights.LoadFromFile(parameters);
                _context = _model.CreateContext(parameters);
                _executor = new InteractiveExecutor(_context);
                _isModelLoaded = true;
                _logger.LogInformation($"AI Model loaded successfully from {modelPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load AI model.");
                _isModelLoaded = false;
            }
        }
        else
        {
            _logger.LogWarning($"AI Model file not found at {modelPath}. AI features will return placeholder text.");
            _isModelLoaded = false;
        }
    }

    public async IAsyncEnumerable<string> GetChatResponseAsync(string systemPrompt, string userMessage)
    {
        if (!_isModelLoaded || _executor == null)
        {
            yield return "System: The AI model is currently unavailable or not configured properly.";
            yield break;
        }

        var inferenceParams = new InferenceParams()
        {
            MaxTokens = 256,
            AntiPrompts = new List<string> { "User:" }
        };

        var prompt = $"System: {systemPrompt}\nUser: {userMessage}\nAssistant:";
        
        await foreach (var text in _executor.InferAsync(prompt, inferenceParams))
        {
            yield return text;
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
        _model?.Dispose();
    }
}
