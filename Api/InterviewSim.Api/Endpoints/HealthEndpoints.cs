namespace InterviewSim.Api.Endpoints;

public static class HealthEndpoints
{
    public static RouteGroupBuilder MapHealthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/health");
        group.MapGet("/ready", () => Results.Ok());
        return group;
    }
}
