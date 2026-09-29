using Microsoft.AspNetCore.Mvc;
using ReviewsAssistant.Application.Reviews;
using ReviewsAssistant.Application.Reviews.Contracts;
using ReviewsAssistant.WebAPI.Contracts.Reviews;

namespace ReviewsAssistant.WebAPI.Controllers;

[ApiController]
[Route("api/reviews")]
public sealed class ReviewsController(IReviewService reviewService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewInput input, CancellationToken cancellationToken)
    {
        var request = new CreateReviewRequest(
            input.AuthorName,
            input.Email,
            input.Text);

        var review = await reviewService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetSubmissionStatus), new { id = review.Id }, review);
    }

    [HttpGet("{id:guid}/submission-status")]
    public async Task<ActionResult<SubmissionStatusDto>> GetSubmissionStatus(Guid id, CancellationToken cancellationToken)
    {
        var status = await reviewService.GetSubmissionStatusAsync(id, cancellationToken);
        return status is null ? NotFound() : Ok(status);
    }
}
