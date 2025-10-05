using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Api.Messaging;
using Infrastructure.Api.Persistence;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Api;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers common infrastructure services such as repositories, unit of work, and Kafka.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string kafkaBootstrap)
    {
        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Kafka producer
        services.AddSingleton<IKafkaProducer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<KafkaProducer>>();
            return new KafkaProducer(logger, kafkaBootstrap);
        });

        return services;
    }
}
