using Azure.AI.OpenAI;
using InterviewSim.Api;

namespace InterviewSim.Core.Tests;

public class FakeOpenAIClient : IOpenAIClient
{
    private readonly ChatCompletions _response;

    public FakeOpenAIClient(ChatCompletions response)
    {
        _response = response;
    }

    public Task<ChatCompletions> GetChatCompletionsAsync(ChatCompletionsOptions options, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_response);
    }
}
