using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.Application.Reviews.Contracts;

public sealed record ReviewQuery(
    AnalysisStatus? Status,
    Sentiment? Sentiment,
    Priority? Priority,
    string? SortBy,
    bool Descending = true);
