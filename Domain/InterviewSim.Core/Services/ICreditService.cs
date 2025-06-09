namespace InterviewSim.Core.Services;

public interface ICreditService
{
    Task<int> GetCreditsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeductCreditsAsync(Guid userId, int amount, CancellationToken cancellationToken = default);
}
