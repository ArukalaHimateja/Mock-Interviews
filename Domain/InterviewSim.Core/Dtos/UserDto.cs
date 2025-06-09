namespace InterviewSim.Core.Dtos;

public readonly record struct UserDto(
    Guid Id,
    string Name,
    int Credits);
