using InterviewSim.Core.Dtos;
using Refit;

namespace InterviewSim.Client.Services;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token);

public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<LoginResponse> Login([Body] LoginRequest request);
}

public interface IQuestionApi
{
    [Get("/api/questions/sample")]
    Task<List<QuestionDto>> GetSampleQuestions();
}
