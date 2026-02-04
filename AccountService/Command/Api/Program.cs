using AccountService.Command.Application.Commands;
using AccountService.Command.Application.Handlers;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using Infrastructure.Api.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("DefaultConnection is missing in configuration.");

builder.Services.AddDbContext<WriteDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register all command handlers automatically from loaded assemblies
builder.Services.AddHandlersFromAssemblies();

// Explicit registration (optionnel si AddHandlersFromAssemblies fonctionne après ajout de la référence)
builder.Services.AddScoped<ICommandHandler<CreateAccountCommand, Guid>, CreateAccountHandler>();

// Register ICommandDispatcher to invoke handlers without registering each handler consumer manually
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();

// Kafka Producer from config
builder.Services.AddSingleton<IKafkaProducer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KafkaProducer>>();
    var bootstrap = builder.Configuration["Kafka:BootstrapServers"];
    if (string.IsNullOrWhiteSpace(bootstrap))
        throw new InvalidOperationException("Kafka:BootstrapServers is missing in configuration.");

    return new KafkaProducer(logger, bootstrap!);
});

var app = builder.Build();

// Swagger in Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();