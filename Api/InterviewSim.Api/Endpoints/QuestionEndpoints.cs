using InterviewSim.Core.Data;
using InterviewSim.Core.Dtos;
using Microsoft.EntityFrameworkCore;

namespace InterviewSim.Api.Endpoints;

public static class QuestionEndpoints
{
    public static RouteGroupBuilder MapQuestionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/questions");

        group.MapGet("/sample", async (InterviewSimContext db) =>
        {
            var questions = await db.Questions
                .Include(q => q.Rubric)
                .OrderBy(q => Guid.NewGuid())
                .Take(3)
                .Select(q => new QuestionDto(
                    q.Id,
                    q.Category,
                    q.Difficulty,
                    q.Prompt,
                    q.ReferenceSolution,
                    q.Rubric.Select(r => new RubricCriterionDto(r.Criterion, r.Weight, r.Excellent, r.Poor)).ToList()))
                .ToListAsync();

            return Results.Ok(questions);
        });

        return group;
    }
}
