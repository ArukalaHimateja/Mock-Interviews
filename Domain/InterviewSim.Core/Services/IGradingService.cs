namespace InterviewSim.Core.Services;

using InterviewSim.Core.Entities;

public interface IGradingService
{
    Task<double> GradeAsync(Question question, string answer, CancellationToken cancellationToken = default);
}
