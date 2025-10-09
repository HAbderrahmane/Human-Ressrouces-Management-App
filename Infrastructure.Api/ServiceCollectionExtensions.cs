using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositories + Unit of Work
        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("WriteDatabase")));

        services.AddDbContext<ReadDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ReadDatabase")));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var kafkaBootstrap = configuration.GetSection("Kafka")["BootstrapServers"] ?? "localhost:9092";

        services.AddSingleton<IKafkaProducer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<KafkaProducer>>();
            return new KafkaProducer(logger, kafkaBootstrap);
        });

        services.AddHostedService(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<KafkaConsumer>>();
            var kafkaConfig = configuration.GetSection("Kafka");
            return new KafkaConsumer(
                kafkaConfig["BootstrapServers"]!,
                kafkaConfig["Topic"] ?? "hrm-events",
                kafkaConfig["GroupId"] ?? "hrm-consumer-group",
                logger
            );
        });

        return services;
    }
}