namespace ReviewsAssistant.Application.Reviews.Contracts;

public sealed record CreateReviewRequest(
    string AuthorName,
    string Email,
    string Text);
