using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Workers;

var builder = Host.CreateApplicationBuilder(args);

// silence EF SQL spam
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

builder.Services.AddDbContext<JobDbContext>(options =>
    options.UseSqlite("Data Source=../jobs.db"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();