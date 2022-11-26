using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Simple.Webhook.Shared;
using Simple.Webhook.Shared.Infra.Kafka;
using System.Text.Json;

namespace Dispatcher.Notification
{
    public class Dispatcher : BackgroundService
    {
        private readonly ILogger<Dispatcher> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly KafkaConfiguration _kafkaConfiguration;
        public Dispatcher(ILogger<Dispatcher> logger, IConsumer<string, string> consumer, IOptions<KafkaConfiguration> options)
        {
            _logger = logger;
            _consumer = consumer;
            _kafkaConfiguration = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _consumer.Subscribe(_kafkaConfiguration.TopicName);
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result is null)
                        continue;

                    var eventNotification = JsonSerializer.Deserialize<Event<object>>(result.Message.Value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}