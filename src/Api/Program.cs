using Application.Abstractions;
using Infrastructure.Queues;
using Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IJobQueue, InMemoryJobQueue>();

builder.Services.AddSingleton<JobService>();

var app = builder.Build();

app.MapGet("/", () => "Git Reproducer API running");

app.Run();