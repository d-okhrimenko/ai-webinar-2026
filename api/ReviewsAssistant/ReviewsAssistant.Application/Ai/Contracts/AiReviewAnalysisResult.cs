using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.Application.Ai.Contracts;

public sealed record AiReviewAnalysisResult(
    Sentiment Sentiment,
    Priority Priority,
    ReviewCategory Category,
    bool NeedsUrgentResponse,
    string Summary,
    string ProviderName,
    string ModelName);
