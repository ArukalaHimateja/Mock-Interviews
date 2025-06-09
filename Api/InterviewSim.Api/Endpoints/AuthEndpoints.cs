using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InterviewSim.Core.Data;
using InterviewSim.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InterviewSim.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth");

        group.MapPost("/login", async (AuthRequest req, InterviewSimContext db, IConfiguration config) =>
        {
            var user = await db.Users.Include(u => u.Credit).FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = req.Email,
                    HashedPassword = req.Password,
                    CreatedUtc = DateTime.UtcNow,
                    Credit = new Credit { Id = Guid.NewGuid(), Balance = 0, UpdatedUtc = DateTime.UtcNow }
                };
                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "secret");
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            });
            var jwt = handler.WriteToken(token);
            return Results.Ok(new { token = jwt });
        });

        return group;
    }

    public record AuthRequest(string Email, string Password);
}
