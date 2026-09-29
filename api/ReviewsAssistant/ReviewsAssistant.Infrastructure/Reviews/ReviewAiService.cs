using Microsoft.EntityFrameworkCore;
using ReviewsAssistant.Application.Ai;
using ReviewsAssistant.Application.Ai.Contracts;
using ReviewsAssistant.Application.Reviews;
using ReviewsAssistant.Application.Reviews.Contracts;
using ReviewsAssistant.Core.Reviews;
using ReviewsAssistant.Infrastructure.Data;

namespace ReviewsAssistant.Infrastructure.Reviews;

public sealed class ReviewAiService(
    ReviewsDbContext dbContext,
    IAiReviewAnalyzer reviewAnalyzer,
    IAiResponseGenerator responseGenerator) : IReviewAiService
{
    public async Task<ReviewDto?> AnalyzeAsync(Guid id, CancellationToken cancellationToken)
    {
        var review = await dbContext.Reviews.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (review is null)
        {
            return null;
        }

        review.AnalysisStatus = AnalysisStatus.Processing;
        try
        {
            var result = await reviewAnalyzer.AnalyzeAsync(new AiReviewAnalysisRequest(review.Text), cancellationToken);
            review.Sentiment = result.Sentiment;
            review.Priority = result.Priority;
            review.Category = result.Category;
            review.NeedsUrgentResponse = result.NeedsUrgentResponse;
            review.Summary = result.Summary;
            review.AnalyzedAtUtc = DateTime.UtcNow;
            review.AnalysisStatus = AnalysisStatus.Completed;
            await dbContext.SaveChangesAsync(cancellationToken);
            return Map(review);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            review.AnalysisStatus = AnalysisStatus.Failed;
            await dbContext.SaveChangesAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<ReviewDto?> GenerateDraftResponseAsync(Guid id, CancellationToken cancellationToken)
    {
        var review = await dbContext.Reviews.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (review is null)
        {
            return null;
        }

        var result = await responseGenerator.GenerateAsync(
            new AiResponseGenerationRequest(review.Text),
            cancellationToken);
        review.AiDraftResponse = result.DraftResponse;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(review);
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
        item.AiDraftResponse,
        item.AnalyzedAtUtc);
}
