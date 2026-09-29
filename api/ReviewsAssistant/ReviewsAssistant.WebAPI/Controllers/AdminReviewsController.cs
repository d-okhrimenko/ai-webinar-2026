using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewsAssistant.Application.Reviews;
using ReviewsAssistant.Application.Reviews.Contracts;
using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.WebAPI.Controllers;

[ApiController, Authorize]
[Route("api/admin/reviews")]
public sealed class AdminReviewsController(IReviewService reviewService) : ControllerBase
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
    public IActionResult Analyze(Guid id) => StatusCode(StatusCodes.Status501NotImplemented,
        new { message = "AI-інтеграцію буде підключено пізніше." });

    [HttpPost("{id:guid}/draft-response")]
    public IActionResult DraftResponse(Guid id) => StatusCode(StatusCodes.Status501NotImplemented,
        new { message = "AI-інтеграцію буде підключено пізніше." });
}
