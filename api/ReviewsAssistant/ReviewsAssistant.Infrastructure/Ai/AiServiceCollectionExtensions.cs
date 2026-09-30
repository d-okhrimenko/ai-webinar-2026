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

        if (string.Equals(provider, "OpenAI", StringComparison.OrdinalIgnoreCase))
        {
            var apiKey = configuration["Ai:OpenAi:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key is required. Configure Ai:OpenAi:ApiKey on the server.");
            }

            var responseInstructionsFile = configuration["Ai:OpenAi:InstructionsFile"]
                ?? "Prompts/OpenAiResponseGeneration.md";
            var analysisInstructionsFile = configuration["Ai:OpenAi:AnalysisInstructionsFile"]
                ?? "Prompts/OpenAiReviewAnalysis.md";
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, responseInstructionsFile)) ||
                !File.Exists(Path.Combine(AppContext.BaseDirectory, analysisInstructionsFile)))
            {
                throw new InvalidOperationException("An OpenAI instructions file was not found.");
            }

            services.AddHttpClient<IAiReviewAnalyzer, OpenAiReviewAnalyzer>(client =>
            {
                client.BaseAddress = new Uri("https://api.openai.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            services.AddHttpClient<IAiResponseGenerator, OpenAiResponseGenerator>(client =>
            {
                client.BaseAddress = new Uri("https://api.openai.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            return services;
        }

        throw new InvalidOperationException(
            $"Unsupported AI provider '{provider ?? "(missing)"}'. Configure Ai:Provider as 'Stub' or 'OpenAI'.");
    }
}
