using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Simple.Webhook.Shared;
using Simple.Webhook.Shared.Infra.Kafka;

namespace Producer.Notification
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IProducer<string, string> _producer;
        private readonly KafkaConfiguration _kafkaConfiguration;
        public Worker(ILogger<Worker> logger, IProducer<string, string> producer, IOptions<KafkaConfiguration> options)
        {
            _logger = logger;
            _producer = producer;
            _kafkaConfiguration = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var eventName = new string[] { "EVENT_A", "EVENT_B", "EVENT_C" };
                var guid = Guid.NewGuid();
                Random random = new Random();
                int randomIndex = random.Next(0, eventName.Length);
                var @event = new Event<object>()
                {
                    Id = guid,
                    Name = eventName[randomIndex],
                    Data = new {
                        Teste = "teste"
                    }
                };
                var json = JsonConvert.SerializeObject(@event);
                _ = _producer.ProduceAsync(_kafkaConfiguration.TopicName, new Message<string, string>()
                {
                    Key = guid.ToString(),
                    Value = json,
                });
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
        }
    }
}