using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ReviewsAssistant.Application.Ai;
using ReviewsAssistant.Application.Ai.Contracts;
using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.Infrastructure.Ai;

public sealed class OpenAiReviewAnalyzer(HttpClient httpClient, IConfiguration configuration) : IAiReviewAnalyzer
{
    private const string ProviderName = "OpenAI";
    private readonly string apiKey = configuration["Ai:OpenAi:ApiKey"]
        ?? throw new InvalidOperationException("OpenAI API key is required.");
    private readonly string modelName = configuration["Ai:OpenAi:Model"] ?? "gpt-5-mini";
    private readonly string instructions = LoadInstructions(configuration);

    public async Task<AiReviewAnalysisResult> AnalyzeAsync(
        AiReviewAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        using var message = new HttpRequestMessage(HttpMethod.Post, "responses")
        {
            Content = JsonContent.Create(new
            {
                model = modelName,
                store = false,
                instructions,
                input = request.ReviewText,
                text = new
                {
                    format = new
                    {
                        type = "json_schema",
                        name = "review_analysis",
                        strict = true,
                        schema = new
                        {
                            type = "object",
                            properties = new
                            {
                                sentiment = new { type = "string", @enum = new[] { "Positive", "Neutral", "Negative" } },
                                priority = new { type = "string", @enum = new[] { "Low", "Medium", "High", "Critical" } },
                                category = new { type = "string", @enum = new[] { "Praise", "FeatureRequest", "Bug", "PaymentIssue", "Support", "Other" } },
                                needsUrgentResponse = new { type = "boolean" },
                                summary = new { type = "string" },
                            },
                            required = new[] { "sentiment", "priority", "category", "needsUrgentResponse", "summary" },
                            additionalProperties = false,
                        },
                    },
                },
            }),
        };
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        using var response = await httpClient.SendAsync(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("OpenAI review analysis request failed.", null, response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken);
        var analysis = ParseAnalysis(ExtractOutputText(document.RootElement));
        var responseModel = document.RootElement.TryGetProperty("model", out var model) && model.ValueKind == JsonValueKind.String
            ? model.GetString()!
            : modelName;

        return new AiReviewAnalysisResult(
            analysis.Sentiment,
            analysis.Priority,
            analysis.Category,
            analysis.NeedsUrgentResponse,
            analysis.Summary,
            ProviderName,
            responseModel);
    }

    private static ParsedAnalysis ParseAnalysis(string outputText)
    {
        using var document = JsonDocument.Parse(outputText);
        var root = document.RootElement;
        var sentiment = ParseEnum<Sentiment>(root, "sentiment");
        var priority = ParseEnum<Priority>(root, "priority");
        var category = ParseEnum<ReviewCategory>(root, "category");
        var needsUrgentResponse = root.GetProperty("needsUrgentResponse").GetBoolean();
        var summary = root.GetProperty("summary").GetString()?.Trim();

        if (string.IsNullOrWhiteSpace(summary) || summary.Length > 1000)
        {
            throw new InvalidOperationException("OpenAI response contained an invalid analysis summary.");
        }

        return new ParsedAnalysis(sentiment, priority, category, needsUrgentResponse, summary);
    }

    private static TEnum ParseEnum<TEnum>(JsonElement root, string propertyName) where TEnum : struct, Enum
    {
        var value = root.GetProperty(propertyName).GetString();
        return Enum.TryParse<TEnum>(value, ignoreCase: false, out var result)
            ? result
            : throw new InvalidOperationException($"OpenAI response contained an invalid {propertyName}.");
    }

    private static string ExtractOutputText(JsonElement response)
    {
        if (!response.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("OpenAI response did not contain output.");
        }

        var text = output.EnumerateArray()
            .Where(item => item.TryGetProperty("type", out var type) && type.GetString() == "message")
            .SelectMany(item => item.GetProperty("content").EnumerateArray())
            .Where(item => item.TryGetProperty("type", out var type) && type.GetString() == "output_text")
            .Select(item => item.GetProperty("text").GetString())
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        return text?.Trim() ?? throw new InvalidOperationException("OpenAI response did not contain analysis text.");
    }

    private static string LoadInstructions(IConfiguration configuration)
    {
        var instructionsFile = configuration["Ai:OpenAi:AnalysisInstructionsFile"]
            ?? "Prompts/OpenAiReviewAnalysis.md";
        var content = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, instructionsFile)).Trim();

        return string.IsNullOrWhiteSpace(content)
            ? throw new InvalidOperationException("OpenAI analysis instructions file must not be empty.")
            : content;
    }

    private sealed record ParsedAnalysis(
        Sentiment Sentiment,
        Priority Priority,
        ReviewCategory Category,
        bool NeedsUrgentResponse,
        string Summary);
}
