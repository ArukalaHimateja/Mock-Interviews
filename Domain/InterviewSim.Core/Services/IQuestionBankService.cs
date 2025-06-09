namespace InterviewSim.Core.Services;

using InterviewSim.Core.Entities;

public interface IQuestionBankService
{
    Task<IReadOnlyList<Question>> GetQuestionsAsync(CancellationToken cancellationToken = default);
}
