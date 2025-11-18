using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Infrastructure.Api;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Middleware;

using SharedKernel.Events;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var kafkaSection = configuration.GetSection("Kafka");

var bootstrapServers = kafkaSection["BootstrapServers"] ?? "localhost:9092";
var topic = kafkaSection["Topic"] ?? "hrm-events";
var groupId = kafkaSection["GroupId"] ?? "hrm-consumer-group";

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IKafkaProducer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KafkaProducer>>();
    return new KafkaProducer(logger, bootstrapServers);
});

builder.Services.AddHostedService(sp =>
    new KafkaConsumer(
        bootstrapServers,
        topic,
        groupId,
        sp.GetRequiredService<ILogger<KafkaConsumer>>()
    )
);

var app = builder.Build();

app.UseGlobalErrorHandler();

app.MapGet("/", () => Results.Ok("Infrastructure.Api is running"));

app.MapPost("/produce", async (IKafkaProducer producer) =>
{
    var evt = new TestEvent();
    var payload = new { Message = "Test message", SentAt = DateTime.UtcNow };
    await producer.ProduceAsync(evt, payload, topic);
    return Results.Ok("Event sent");
});

app.Run();

public class TestEvent : BaseEvent { }
