using Infrastructure.Api;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var configuration = builder.Configuration;
var kafkaSection = configuration.GetSection("Kafka");

var bootstrapServers = kafkaSection["BootstrapServers"] ?? "localhost:9092";
var topic = kafkaSection["Topic"] ?? "hrm-events";
var groupId = kafkaSection["GroupId"] ?? "hrm-consumer-group";

// Register dependencies
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Error handling middleware
app.UseGlobalErrorHandler();

app.MapGet("/", () => Results.Ok("Infrastructure.Api is running ✅"));

app.MapGet("/error", () =>
{
    throw new Exception("Simulated test exception");
});

app.MapPost("/produce", async (IKafkaProducer producer) =>
{
    var message = $"Test message at {DateTime.UtcNow:O}";
    await producer.ProduceAsync(topic, message);
    return Results.Ok(new { sent = message });
});

app.Run();