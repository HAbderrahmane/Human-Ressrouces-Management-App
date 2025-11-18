using Infrastructure.Api.Messaging;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Kafka config
var kafkaSection = builder.Configuration.GetSection("Kafka");
var bootstrap = kafkaSection["BootstrapServers"]!;
var topic = kafkaSection["Topic"]!;
var group = kafkaSection["GroupId"]!;

// Register Kafka generic consumer
builder.Services.AddHostedService(sp =>
    new KafkaConsumer(
        bootstrap,
        topic,
        group,
        sp.GetRequiredService<ILogger<KafkaConsumer>>()
    )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
