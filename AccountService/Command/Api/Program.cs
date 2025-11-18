using AccountService.Command.Application.Handlers;
using Infrastructure.Api.Messaging;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR v13 registration
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateAccountHandler).Assembly);
});

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
