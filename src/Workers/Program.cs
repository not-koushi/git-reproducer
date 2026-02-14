using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Workers;

var builder = Host.CreateApplicationBuilder(args);

// IMPORTANT: same database file as API
builder.Services.AddDbContext<JobDbContext>(options =>
    options.UseSqlite("Data Source=../jobs.db"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();