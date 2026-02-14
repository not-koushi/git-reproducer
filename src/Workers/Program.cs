using Application.Abstractions;
using Infrastructure.Queues;
using Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IJobQueue, InMemoryJobQueue>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();