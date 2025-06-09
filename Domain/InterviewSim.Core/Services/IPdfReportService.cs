namespace InterviewSim.Core.Services;

using InterviewSim.Core.Entities;

public interface IPdfReportService
{
    Task<string> GenerateReportAsync(InterviewSession session, CancellationToken cancellationToken = default);
}
