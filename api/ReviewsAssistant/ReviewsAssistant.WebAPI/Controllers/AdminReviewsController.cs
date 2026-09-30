using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewsAssistant.Application.Reviews;
using ReviewsAssistant.Application.Reviews.Contracts;
using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.WebAPI.Controllers;

[ApiController, Authorize]
[Route("api/admin/reviews")]
public sealed class AdminReviewsController(
    IReviewService reviewService,
    IReviewAiService reviewAiService) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ReviewDto>> GetAll([FromQuery] AnalysisStatus? status, [FromQuery] Sentiment? sentiment,
        [FromQuery] Priority? priority, [FromQuery] string? sortBy, [FromQuery] bool descending = true,
        CancellationToken cancellationToken = default) =>
        reviewService.GetAllAsync(new ReviewQuery(status, sentiment, priority, sortBy, descending), cancellationToken);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReviewDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var review = await reviewService.GetByIdAsync(id, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpPost("{id:guid}/analyze")]
    public async Task<ActionResult<ReviewDto>> Analyze(Guid id, CancellationToken cancellationToken)
    {
        var review = await reviewAiService.AnalyzeAsync(id, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpPost("{id:guid}/draft-response")]
    public async Task<ActionResult<ReviewDto>> DraftResponse(Guid id, CancellationToken cancellationToken)
    {
        var review = await reviewAiService.GenerateDraftResponseAsync(id, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }
}
