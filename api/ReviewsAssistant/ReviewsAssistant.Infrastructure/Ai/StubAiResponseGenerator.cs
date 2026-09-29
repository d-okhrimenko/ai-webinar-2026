using ReviewsAssistant.Application.Ai;
using ReviewsAssistant.Application.Ai.Contracts;

namespace ReviewsAssistant.Infrastructure.Ai;

public sealed class StubAiResponseGenerator : IAiResponseGenerator
{
    private const string ProviderName = "Stub";
    private const string ModelName = "deterministic-v1";

    public Task<AiResponseGenerationResult> GenerateAsync(
        AiResponseGenerationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new AiResponseGenerationResult(
            "Дякуємо за ваш відгук. Ми передали його команді та зв’яжемося з вами за потреби.",
            ProviderName,
            ModelName));
    }
}
