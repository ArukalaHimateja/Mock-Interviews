using System.Text.Json;
using Azure.AI.OpenAI;
using InterviewSim.Api;
using InterviewSim.Core.Dtos;
using InterviewSim.Core.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InterviewSim.Core.Tests;

public class GradingServiceTests
{
    [Fact]
    public async Task GradeAsync_Computes_Weighted_Score()
    {
        var question = new Question
        {
            ReferenceSolution = "ref",
            Rubric = new()
            {
                new RubricCriterion { Criterion = "a", Weight = 0.5, Excellent = "", Poor = "" },
                new RubricCriterion { Criterion = "b", Weight = 0.5, Excellent = "", Poor = "" }
            }
        };

        var scores = new[] { new RubricScore("a", 1.0), new RubricScore("b", 0.5) };
        var json = JsonSerializer.Serialize(scores);

        var toolCall = new ChatCompletionsFunctionToolCall("1", "record_scores", json);
        var message = new ChatResponseMessage(ChatRole.Assistant);
        message.ToolCalls.Add(toolCall);
        var choice = new ChatChoice(message);
        var completions = new ChatCompletions();
        completions.Choices.Add(choice);
        completions.Usage = new CompletionsUsage(10, 10, 20);

        var fakeClient = new FakeOpenAIClient(completions);
        var baseDir = AppContext.BaseDirectory;
        var root = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", ".."));
        var env = new FakeEnvironment { ContentRootPath = Path.Combine(root, "Api") };
        var service = new GradingService(fakeClient, env, NullLogger<GradingService>.Instance);

        var result = await service.GradeAsync(question, "ans");

        Assert.Equal(0.75, result, 2);
    }
}
