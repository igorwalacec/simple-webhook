using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Simple.Webhook.Shared.Infra.Kafka;
public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfiguration = new KafkaConfiguration();
        configuration.Bind("KafkaConfiguration", kafkaConfiguration);
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaConfiguration.BootstrapServer,
            GroupId = "simple.webhook",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SecurityProtocol = SecurityProtocol.Plaintext
        };
        services.AddSingleton<IConsumer<string, string>>(_ => new ConsumerBuilder<string, string>(config).Build());

        return services;
    }
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfiguration = new KafkaConfiguration();
        configuration.Bind("KafkaConfiguration", kafkaConfiguration);
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaConfiguration.BootstrapServer
        };

        services.AddSingleton<IProducer<string, string>>(_ => new ProducerBuilder<string, string>(config).Build());

        return services;
    }
}
