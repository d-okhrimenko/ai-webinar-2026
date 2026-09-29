using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.Application.Reviews.Contracts;

public sealed record SubmissionStatusDto(
    Guid Id,
    AnalysisStatus AnalysisStatus,
    DateTime CreatedAtUtc);
