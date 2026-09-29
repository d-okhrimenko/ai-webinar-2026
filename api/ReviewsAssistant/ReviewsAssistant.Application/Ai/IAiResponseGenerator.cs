using ReviewsAssistant.Application.Ai.Contracts;

namespace ReviewsAssistant.Application.Ai;

public interface IAiResponseGenerator
{
    Task<AiResponseGenerationResult> GenerateAsync(
        AiResponseGenerationRequest request,
        CancellationToken cancellationToken);
}
