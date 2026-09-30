using ReviewsAssistant.Application.Reviews.Contracts;

namespace ReviewsAssistant.Application.Reviews;

public interface IReviewService
{
    Task<ReviewDto> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken);

    Task<SubmissionStatusDto?> GetSubmissionStatusAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ReviewDto>> GetAllAsync(ReviewQuery query, CancellationToken cancellationToken);

    Task<ReviewDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
