using Application.Abstractions;
using Infrastructure.Queues;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IJobQueue, InMemoryJobQueue>();

var app = builder.Build();

app.MapGet("/", () => "Git Reproducer API running");

app.Run();