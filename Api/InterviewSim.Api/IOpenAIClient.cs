using Azure.AI.OpenAI;

namespace InterviewSim.Api;

public interface IOpenAIClient
{
    Task<ChatCompletions> GetChatCompletionsAsync(ChatCompletionsOptions options, CancellationToken cancellationToken = default);
}
