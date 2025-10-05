using Infrastructure.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register MVC or minimal endpoints for test
builder.Services.AddControllers();

var app = builder.Build();

// Use your custom error-handling middleware
app.UseGlobalErrorHandler();

// Basic test endpoint
app.MapGet("/", () => "Infrastructure.Api is running");

// Endpoint that throws
app.MapGet("/error", () =>
{
    throw new Exception("Simulated test exception");
});

// Run the app
app.Run();
