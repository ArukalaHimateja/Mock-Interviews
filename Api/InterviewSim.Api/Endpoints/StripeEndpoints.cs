using InterviewSim.Core.Data;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace InterviewSim.Api.Endpoints;

public static class StripeEndpoints
{
    public static RouteGroupBuilder MapStripeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/stripe");

        group.MapPost("/webhook", async (HttpRequest request, InterviewSimContext db) =>
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            var evt = EventUtility.ParseEvent(json);
            if (evt.Type == Events.PaymentIntentSucceeded)
            {
                var user = await db.Users.Include(u => u.Credit).FirstOrDefaultAsync();
                if (user?.Credit != null)
                {
                    user.Credit.Balance += 1;
                    user.Credit.UpdatedUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                }
            }
            return Results.Ok();
        });

        return group;
    }
}
