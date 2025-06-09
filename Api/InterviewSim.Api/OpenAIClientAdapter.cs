using Azure.AI.OpenAI;

namespace InterviewSim.Api;

public class OpenAIClientAdapter : IOpenAIClient
{
    private readonly OpenAIClient _client;
    private const string Deployment = "gpt-4o";

    public OpenAIClientAdapter(OpenAIClient client)
    {
        _client = client;
    }

    public async Task<ChatCompletions> GetChatCompletionsAsync(ChatCompletionsOptions options, CancellationToken cancellationToken = default)
    {
        var resp = await _client.GetChatCompletionsAsync(Deployment, options, cancellationToken);
        return resp.Value;
    }
}
