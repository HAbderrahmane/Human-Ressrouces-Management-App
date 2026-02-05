using AccountService.Query.Infrastructure;
using Infrastructure.Api.Extensions;
using Infrastructure.Api.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create a temporary logger factory for the initialization phase
var loggerFactory = LoggerFactory.Create(lb => lb.AddConsole());
var initLogger = loggerFactory.CreateLogger<Program>();

ReadDbContext? readDbContext = null;
try
{
    // Initialize MongoDB read context
    readDbContext = new ReadDbContext(builder.Configuration);
    initLogger.LogInformation("MongoDB ReadDbContext initialized");
}
catch (Exception ex)
{
    // Log initialization failure
    initLogger.LogError(ex, "MongoDB initialization failed: {Message}", ex.Message);
}

if (readDbContext != null)
{
    // Register the read DB context as a singleton if initialization succeeded
    builder.Services.AddSingleton(readDbContext);
}
else
{
    // Warn that MongoDB is not available and the application will continue without it
    initLogger.LogWarning("MongoDB not available, using null context");
}

// Register all event handlers automatically from loaded assemblies
builder.Services.AddHandlersFromAssemblies();

// Resolve kafka configuration
var kafkaSection = builder.Configuration.GetSection("Kafka");
var bootstrap = kafkaSection["BootstrapServers"] ?? "localhost:29092";
var topic = kafkaSection["Topic"] ?? "account.events";
var group = kafkaSection["GroupId"] ?? $"account-query-{Guid.NewGuid()}";

// Register a hosted Kafka consumer which will dispatch events to discovered handlers
builder.Services.AddHostedService(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KafkaConsumer>>();
    logger.LogInformation("Kafka consumer config: Bootstrap={Bootstrap}, Topic={Topic}, Group={Group}", bootstrap, topic, group);

    var consumer = new KafkaConsumer(bootstrap, topic, group, logger);

    // Discover event handler types by reflection and register delegates that create a scope
    var handlerInterface = typeof(IEventHandler);
    var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a =>
        {
            try { return a.GetTypes(); }
            catch { return Array.Empty<Type>(); }
        })
        .Where(t => handlerInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
        .ToList();

    logger.LogInformation("Discovered {Count} event handler(s):", handlerTypes.Count);
    foreach (var ht in handlerTypes)
    {
        logger.LogInformation("  - {HandlerType}", ht.FullName);
    }

    foreach (var ht in handlerTypes)
    {
        // Create a delegate that resolves the concrete handler from a scope and invokes it
        consumer.RegisterHandler(
            ResolveEventTypeForRegistration(sp, ht),
            async (payload) =>
            {
                try
                {
                    using var scope = sp.CreateScope();
                    var handler = (IEventHandler)scope.ServiceProvider.GetRequiredService(ht);
                    await handler.HandleAsync(payload);
                }
                catch (Exception ex)
                {
                    var logger2 = sp.GetRequiredService<ILogger<Program>>();
                    logger2.LogError(ex, "Error handling event by {HandlerType}: {Message}", ht.FullName, ex.Message);
                }
            });
    }

    return consumer;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

var appLogger = app.Services.GetRequiredService<ILogger<Program>>();
appLogger.LogInformation("Application started on {Environment} environment", app.Environment.EnvironmentName);

app.Run();

static string ResolveEventTypeForRegistration(IServiceProvider sp, Type handlerType)
{
    using var scope = sp.CreateScope();
    var inst = (IEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);
    return inst.EventType;
}