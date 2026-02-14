using Application.Abstractions;
using Infrastructure.Queues;
using Application.Services;
using Application.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IJobQueue, InMemoryJobQueue>();

builder.Services.AddSingleton<JobService>();

var app = builder.Build();

app.MapGet("/", () => "Git Reproducer API running");

app.MapPost("/jobs", (CreateJobRequest request, JobService service) =>
{
    var job = service.Create(request);
    return Results.Ok(job);
});

app.Run();