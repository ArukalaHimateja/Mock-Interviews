using InterviewSim.Core.Entities;
using InterviewSim.Core.Services;

namespace InterviewSim.Api;

public class DummyGradingService : IGradingService
{
    public Task<double> GradeAsync(Question question, string answer, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1.0);
    }
}
