using ReviewsAssistant.Application.Ai;
using ReviewsAssistant.Application.Ai.Contracts;
using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.Infrastructure.Ai;

public sealed class StubAiReviewAnalyzer : IAiReviewAnalyzer
{
    private const string ProviderName = "Stub";
    private const string ModelName = "deterministic-v1";

    public Task<AiReviewAnalysisResult> AnalyzeAsync(
        AiReviewAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedText = request.ReviewText.Trim().ToLowerInvariant();
        var isPaymentIssue = ContainsAny(normalizedText, "payment", "refund", "invoice", "charge", "оплат", "повернен");
        var isBug = ContainsAny(normalizedText, "bug", "error", "broken", "crash", "помил", "не працю");
        var isSupport = ContainsAny(normalizedText, "support", "help", "підтрим", "допомог");
        var isFeatureRequest = ContainsAny(normalizedText, "feature", "request", "would like", "хотілося", "додайте");
        var isUrgent = ContainsAny(normalizedText, "urgent", "immediately", "asap", "терміново", "негайно");
        var isNegative = isPaymentIssue || isBug || ContainsAny(normalizedText, "bad", "terrible", "disappointed", "погано", "жахливо");
        var isPositive = ContainsAny(normalizedText, "great", "excellent", "thank", "love", "чудово", "дякую");

        var category = isPaymentIssue
            ? ReviewCategory.PaymentIssue
            : isBug
                ? ReviewCategory.Bug
                : isSupport
                    ? ReviewCategory.Support
                    : isFeatureRequest
                        ? ReviewCategory.FeatureRequest
                        : isPositive
                            ? ReviewCategory.Praise
                            : ReviewCategory.Other;
        var priority = isUrgent || isPaymentIssue ? Priority.Critical : isBug || isNegative ? Priority.High : Priority.Medium;
        var sentiment = isNegative ? Sentiment.Negative : isPositive ? Sentiment.Positive : Sentiment.Neutral;

        return Task.FromResult(new AiReviewAnalysisResult(
            sentiment,
            priority,
            category,
            isUrgent,
            "Тестовий аналіз відгуку сформовано детермінованою заглушкою.",
            ProviderName,
            ModelName));
    }

    private static bool ContainsAny(string text, params string[] terms) =>
        terms.Any(text.Contains);
}
