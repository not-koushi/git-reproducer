using Application.Abstractions;
using Application.Contracts;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<JobDbContext>(options =>
    options.UseSqlite("Data Source=jobs.db"));

builder.Services.AddScoped<IJobRepository, SqliteJobRepository>();
builder.Services.AddScoped<JobService>();

var app = builder.Build();

app.MapGet("/", () => "Git Reproducer API running");

app.MapPost("/jobs", async (CreateJobRequest request, JobService service) =>
{
    var job = await service.CreateAsync(request);
    return Results.Ok(job);
});

app.Run();