using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Dispatcher.Notification.DTOs;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.Extensions.Options;

namespace Dispatcher.Notification;
public class TopicInitializer : IAsyncInitializer
{
    private readonly KafkaConfiguration kafkaConfiguration;
    public TopicInitializer(IOptions<KafkaConfiguration> options)
    {
        kafkaConfiguration = options.Value;
    }
        
    public async Task InitializeAsync()
    {
        using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = kafkaConfiguration.BootstrapServer }).Build())
        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));

            if (!metadata.Topics.Where(x => x.Topic == kafkaConfiguration.TopicName).Any())
            {
                var newTopic = new TopicSpecification
                {
                    Name = kafkaConfiguration.TopicName,
                    NumPartitions = kafkaConfiguration.MinPartition,
                    ReplicationFactor = 1
                };
                await adminClient.CreateTopicsAsync(new[] { newTopic });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured creating topic");
        }
    }
}