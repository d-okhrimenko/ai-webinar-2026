using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ReviewsAssistant.Application.Ai;
using ReviewsAssistant.Application.Ai.Contracts;

namespace ReviewsAssistant.Infrastructure.Ai;

public sealed class OpenAiResponseGenerator(HttpClient httpClient, IConfiguration configuration) : IAiResponseGenerator
{
    private const string ProviderName = "OpenAI";
    private readonly string apiKey = configuration["Ai:OpenAi:ApiKey"]
        ?? throw new InvalidOperationException("OpenAI API key is required.");
    private readonly string modelName = configuration["Ai:OpenAi:Model"] ?? "gpt-5-mini";
    private readonly string instructions = LoadInstructions(configuration);

    public async Task<AiResponseGenerationResult> GenerateAsync(
        AiResponseGenerationRequest request,
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
            }),
        };
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        using var response = await httpClient.SendAsync(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("OpenAI response generation request failed.", null, response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(contentStream, cancellationToken: cancellationToken);
        var draftResponse = ExtractOutputText(document.RootElement);
        var responseModel = document.RootElement.TryGetProperty("model", out var model)
            && model.ValueKind == JsonValueKind.String
            ? model.GetString()!
            : modelName;

        return new AiResponseGenerationResult(draftResponse, ProviderName, responseModel);
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

        return text?.Trim() ?? throw new InvalidOperationException("OpenAI response did not contain generated text.");
    }

    private static string LoadInstructions(IConfiguration configuration)
    {
        var instructionsFile = configuration["Ai:OpenAi:InstructionsFile"]
            ?? "Prompts/OpenAiResponseGeneration.md";
        var path = Path.Combine(AppContext.BaseDirectory, instructionsFile);
        var content = File.ReadAllText(path).Trim();

        return string.IsNullOrWhiteSpace(content)
            ? throw new InvalidOperationException("OpenAI instructions file must not be empty.")
            : content;
    }
}
