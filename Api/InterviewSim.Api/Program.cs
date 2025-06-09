using System.Text;
using Azure.AI.OpenAI;
using Azure.Storage.Blobs;
using InterviewSim.Api.Endpoints;
using InterviewSim.Api;
using InterviewSim.Core.Data;
using InterviewSim.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InterviewSimContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton(_ => new StripeClient(builder.Configuration["Stripe:SecretKey"] ?? string.Empty));
builder.Services.AddSingleton(_ => new OpenAIClient(
    new Uri(builder.Configuration["OpenAI:Endpoint"] ?? "http://localhost"),
    new AzureKeyCredential(builder.Configuration["OpenAI:ApiKey"] ?? string.Empty)));
builder.Services.AddSingleton(_ => new BlobServiceClient(
    builder.Configuration.GetConnectionString("Storage") ?? "UseDevelopmentStorage=true"));

builder.Services.AddSingleton<IGradingService, DummyGradingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "InterviewSim API", Version = "v1" });
    var jwtScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    o.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<InterviewSimContext>();
    ctx.Database.EnsureCreated();
    var dataPath = Path.Combine(app.Environment.ContentRootPath, "..", "Data", "questions_v1.json");
    await ctx.MigrateAndSeedAsync(dataPath);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api");
api.MapAuthEndpoints();
api.MapQuestionEndpoints();
api.MapSessionEndpoints();
api.MapStripeEndpoints();
api.MapHealthEndpoints();

app.MapGet("/", () => "Hello from InterviewSim API");

app.Run();
