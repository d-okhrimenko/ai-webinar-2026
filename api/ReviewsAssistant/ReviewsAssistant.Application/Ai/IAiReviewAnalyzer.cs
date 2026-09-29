using ReviewsAssistant.Application.Ai.Contracts;

namespace ReviewsAssistant.Application.Ai;

public interface IAiReviewAnalyzer
{
    Task<AiReviewAnalysisResult> AnalyzeAsync(
        AiReviewAnalysisRequest request,
        CancellationToken cancellationToken);
}
