namespace ReviewsAssistant.Core.Reviews;

public sealed class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AuthorName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public AnalysisStatus AnalysisStatus { get; set; } = AnalysisStatus.Pending;
    public Sentiment? Sentiment { get; set; }
    public Priority? Priority { get; set; }
    public ReviewCategory? Category { get; set; }
    public bool? NeedsUrgentResponse { get; set; }
    public string? Summary { get; set; }
    public string? AiDraftResponse { get; set; }
    public string? AiDraftResponseProvider { get; set; }
    public string? AiDraftResponseModel { get; set; }
    public DateTime? AnalyzedAtUtc { get; set; }
}

public enum AnalysisStatus { Pending, Processing, Completed, Failed }
public enum Sentiment { Positive, Neutral, Negative }
public enum Priority { Low, Medium, High, Critical }
public enum ReviewCategory { Praise, FeatureRequest, Bug, PaymentIssue, Support, Other }
