using System.ComponentModel.DataAnnotations;

namespace ReviewsAssistant.WebAPI.Contracts.Auth;

public sealed class LoginInput
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
