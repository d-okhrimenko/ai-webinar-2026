using ReviewsAssistant.Application.Reviews.Contracts;

namespace ReviewsAssistant.Application.Reviews;

public interface IReviewAiService
{
    Task<ReviewDto?> AnalyzeAsync(Guid id, CancellationToken cancellationToken);

    Task<ReviewDto?> GenerateDraftResponseAsync(Guid id, CancellationToken cancellationToken);
}
