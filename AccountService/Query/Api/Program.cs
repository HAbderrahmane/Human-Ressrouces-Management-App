using Infrastructure.Api.Messaging;
using AccountService.Query.Infrastructure;
using AccountService.Query.Application.Consumers;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ReadDbContext? readDbContext = null;
try
{
    readDbContext = new ReadDbContext(builder.Configuration);
    Console.WriteLine("MongoDB ReadDbContext initialized");
}
catch (Exception ex)
{
    Console.WriteLine($"MongoDB initialization failed: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
}

if (readDbContext != null)
{
    builder.Services.AddSingleton(readDbContext);
}
else
{
    Console.WriteLine("MongoDB not available, using null context");
}

builder.Services.AddScoped<AccountEventConsumer>();

var kafkaSection = builder.Configuration.GetSection("Kafka");
var bootstrap = kafkaSection["BootstrapServers"]!;
var topic = kafkaSection["Topic"]!;
var group = kafkaSection["GroupId"]!;

builder.Services.AddHostedService(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KafkaConsumer>>();
    var consumer = new KafkaConsumer(bootstrap, topic, group, logger);

    consumer.RegisterHandler("AccountService.Command.Domain.Events.AccountCreatedEvent",
        async (payload) =>
        {
            try
            {
                using (var scope = sp.CreateScope())
                {
                    var eventConsumer = scope.ServiceProvider.GetRequiredService<AccountEventConsumer>();
                    await eventConsumer.HandleAccountCreatedAsync(payload);
                }
            }
        });

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
app.Run();
