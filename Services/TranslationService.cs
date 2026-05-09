using GTranslate.Translators;

namespace CompanyDashboardAPI.Services;

public interface ITranslationService
{
    Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage = "auto");
}

public class TranslationService : ITranslationService
{
    private readonly AggregateTranslator _translator;
    private readonly ILogger<TranslationService> _logger;

    public TranslationService(ILogger<TranslationService> logger)
    {
        _translator = new AggregateTranslator();
        _logger = logger;
    }

    public async Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage = "auto")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var result = await _translator.TranslateAsync(text, targetLanguage, sourceLanguage == "auto" ? null : sourceLanguage);
            return result.Translation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Translation failed for text: {text}");
            return text; // Return original text if translation fails
        }
    }
}
