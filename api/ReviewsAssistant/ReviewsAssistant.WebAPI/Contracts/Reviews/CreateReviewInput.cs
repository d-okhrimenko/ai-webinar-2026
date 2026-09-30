using System.ComponentModel.DataAnnotations;

namespace ReviewsAssistant.WebAPI.Contracts.Reviews;

public sealed class CreateReviewInput
{
    [Required]
    [StringLength(150)]
    public string AuthorName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(4000, MinimumLength = 3)]
    public string Text { get; init; } = string.Empty;
}
