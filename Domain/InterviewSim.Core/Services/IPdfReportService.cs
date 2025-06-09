namespace InterviewSim.Core.Services;

using InterviewSim.Core.Entities;

public interface IPdfReportService
{
    Task<byte[]> GenerateReportAsync(InterviewSession session, CancellationToken cancellationToken = default);
}
