using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewsAssistant.Application.Ai;

namespace ReviewsAssistant.Infrastructure.Ai;

public static class AiServiceCollectionExtensions
{
    public static IServiceCollection AddAiProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Ai:Provider"];
        if (string.Equals(provider, "Stub", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IAiReviewAnalyzer, StubAiReviewAnalyzer>();
            services.AddScoped<IAiResponseGenerator, StubAiResponseGenerator>();
            return services;
        }

        throw new InvalidOperationException(
            $"Unsupported AI provider '{provider ?? "(missing)"}'. Configure Ai:Provider as 'Stub'.");
    }
}
