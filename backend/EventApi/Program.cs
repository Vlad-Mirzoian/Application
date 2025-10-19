using Microsoft.EntityFrameworkCore;
using EventApi.Data;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load("../../.env");

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenApi();

var connectionStringTemplate = builder.Configuration.GetConnectionString("Default");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASS");
var connectionString = connectionStringTemplate?.Replace("${DB_PASS}", dbPassword);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();