using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.Application.Reviews.Contracts;

public sealed record ReviewDto(
    Guid Id,
    string AuthorName,
    string Email,
    string Text,
    DateTime CreatedAtUtc,
    AnalysisStatus AnalysisStatus,
    Sentiment? Sentiment,
    Priority? Priority,
    ReviewCategory? Category,
    bool? NeedsUrgentResponse,
    string? Summary,
    string? AiAnalysisProvider,
    string? AiAnalysisModel,
    string? AiDraftResponse,
    string? AiDraftResponseProvider,
    string? AiDraftResponseModel,
    DateTime? AnalyzedAtUtc);
