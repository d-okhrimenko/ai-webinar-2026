using Microsoft.EntityFrameworkCore;
using ReviewsAssistant.Application.Reviews;
using ReviewsAssistant.Application.Reviews.Contracts;
using ReviewsAssistant.Core.Reviews;
using ReviewsAssistant.Infrastructure.Data;

namespace ReviewsAssistant.Infrastructure.Reviews;

public sealed class ReviewService(ReviewsDbContext dbContext) : IReviewService
{
    public async Task<ReviewDto> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var review = new Review
        {
            AuthorName = request.AuthorName.Trim(),
            Email = request.Email.Trim(),
            Text = request.Text.Trim(),
        };

        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(review);
    }

    public async Task<SubmissionStatusDto?> GetSubmissionStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new SubmissionStatusDto(item.Id, item.AnalysisStatus, item.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReviewDto>> GetAllAsync(ReviewQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Review> reviews = dbContext.Reviews.AsNoTracking();
        if (query.Status is not null)
        {
            reviews = reviews.Where(item => item.AnalysisStatus == query.Status);
        }

        if (query.Sentiment is not null)
        {
            reviews = reviews.Where(item => item.Sentiment == query.Sentiment);
        }

        if (query.Priority is not null)
        {
            reviews = reviews.Where(item => item.Priority == query.Priority);
        }

        reviews = query.SortBy?.ToLowerInvariant() switch
        {
            "priority" => query.Descending
                ? reviews.OrderByDescending(item => item.Priority)
                : reviews.OrderBy(item => item.Priority),
            _ => query.Descending
                ? reviews.OrderByDescending(item => item.CreatedAtUtc)
                : reviews.OrderBy(item => item.CreatedAtUtc),
        };

        return await reviews
            .Select(item => Map(item))
            .ToListAsync(cancellationToken);
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => Map(item))
            .SingleOrDefaultAsync(cancellationToken);
    }

    private static ReviewDto Map(Review item) => new(
        item.Id,
        item.AuthorName,
        item.Email,
        item.Text,
        item.CreatedAtUtc,
        item.AnalysisStatus,
        item.Sentiment,
        item.Priority,
        item.Category,
        item.NeedsUrgentResponse,
        item.Summary,
        item.AiAnalysisProvider,
        item.AiAnalysisModel,
        item.AiDraftResponse,
        item.AiDraftResponseProvider,
        item.AiDraftResponseModel,
        item.AnalyzedAtUtc);
}
