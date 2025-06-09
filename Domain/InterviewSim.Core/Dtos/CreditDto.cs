namespace InterviewSim.Core.Dtos;

public readonly record struct CreditDto(
    Guid UserId,
    int Balance);
