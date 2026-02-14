using Application.Abstractions;
using Infrastructure.Queues;
using Application.Services;
using Application.Contracts;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IJobQueue, InMemoryJobQueue>();

builder.Services.AddSingleton<JobService>();

builder.Services.AddDbContext<JobDbContext>(options =>
    options.UseSqlite("Data source = jobs.db"));

var app = builder.Build();

app.MapGet("/", () => "Git Reproducer API running");

app.MapPost("/jobs", (CreateJobRequest request, JobService service) =>
{
    var job = service.Create(request);
    return Results.Ok(job);
});

app.Run();