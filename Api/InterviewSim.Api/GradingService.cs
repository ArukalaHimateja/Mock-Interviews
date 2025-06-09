using System.Text.Json;
using Azure.AI.OpenAI;
using InterviewSim.Core.Dtos;
using InterviewSim.Core.Entities;
using InterviewSim.Core.Services;
using Microsoft.Extensions.Logging;

namespace InterviewSim.Api;

public class GradingService : IGradingService
{
    private readonly IOpenAIClient _client;
    private readonly ILogger<GradingService> _logger;
    private readonly string _systemPrompt;
    private readonly string _userPrompt;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };
    private const double PricePer1K = 0.01;

    public GradingService(IOpenAIClient client, IWebHostEnvironment env, ILogger<GradingService> logger)
    {
        _client = client;
        _logger = logger;
        var dir = Path.Combine(env.ContentRootPath, "..", "Prompts");
        _systemPrompt = File.ReadAllText(Path.Combine(dir, "system.txt"));
        _userPrompt = File.ReadAllText(Path.Combine(dir, "user.txt"));
    }

    public async Task<double> GradeAsync(Question question, string answer, CancellationToken cancellationToken = default)
    {
        var rubricJson = JsonSerializer.Serialize(question.Rubric.Select(r => new { r.Criterion, r.Weight, r.Excellent, r.Poor }), _json);
        var user = _userPrompt.Replace("{reference}", question.ReferenceSolution ?? string.Empty)
            .Replace("{rubric}", rubricJson)
            .Replace("{answer}", answer);

        var options = new ChatCompletionsOptions();
        options.Messages.Add(new ChatRequestSystemMessage(_systemPrompt));
        options.Messages.Add(new ChatRequestUserMessage(user));
        options.Tools.Add(new ChatCompletionsFunctionToolDefinition(new FunctionDefinition
        {
            Name = "record_scores",
            Description = "Return rubric scores",
            Parameters = BinaryData.FromObjectAsJson(new
            {
                type = "object",
                properties = new
                {
                    scores = new
                    {
                        type = "array",
                        items = new
                        {
                            type = "object",
                            properties = new { criterion = new { type = "string" }, score = new { type = "number" } },
                            required = new[] { "criterion", "score" }
                        }
                    }
                },
                required = new[] { "scores" }
            })
        }));

        ChatCompletions completion = null!;
        var delay = TimeSpan.FromSeconds(1);
        for (var attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                completion = await _client.GetChatCompletionsAsync(options, cancellationToken);
                break;
            }
            catch when (attempt < 2)
            {
                await Task.Delay(delay, cancellationToken);
                delay *= 2;
            }
        }

        if (completion.Usage is not null)
        {
            var tokens = completion.Usage.PromptTokens + completion.Usage.CompletionTokens;
            var cost = tokens / 1000d * PricePer1K;
            _logger.LogInformation("Grading cost {Tokens} tokens = ${Cost:F4}", tokens, cost);
        }

        var call = (ChatCompletionsFunctionToolCall)completion.Choices[0].Message.ToolCalls[0];
        var scores = JsonSerializer.Deserialize<RubricScore[]>(call.Arguments, _json) ?? Array.Empty<RubricScore>();

        double total = 0;
        foreach (var r in question.Rubric)
        {
            var s = scores.FirstOrDefault(x => x.Criterion.Equals(r.Criterion, StringComparison.OrdinalIgnoreCase));
            if (s != null) total += s.Score * r.Weight;
        }
        return total;
    }
}
