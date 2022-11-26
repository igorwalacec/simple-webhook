using Confluent.Kafka;
using Dispatcher.Notification.Repositories.Interfaces;
using Dispatcher.Notification.Resilience;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Wrap;
using Simple.Webhook.Shared;
using Simple.Webhook.Shared.Infra.Kafka;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dispatcher.Notification
{
    public class Dispatcher : BackgroundService
    {
        private readonly ILogger<Dispatcher> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly KafkaConfiguration _kafkaConfiguration;
        private IWebhookConfigurationRepository _webhookConfigurationRepository;
        private readonly IAsyncPolicy<HttpResponseMessage> _defaultPolicy;
        private readonly IPolicyRegistry<string> _policies;
        public Dispatcher(ILogger<Dispatcher> logger, IConsumer<string, string> consumer,
            IOptions<KafkaConfiguration> options, IWebhookConfigurationRepository webhookConfigurationRepository,
            IPolicyRegistry<string> policies)
        {
            _logger = logger;
            _consumer = consumer;
            _kafkaConfiguration = options.Value;
            _webhookConfigurationRepository = webhookConfigurationRepository;
            _defaultPolicy = policies.Get<IAsyncPolicy<HttpResponseMessage>>("default");
            _policies = policies;
        }
        public async Task DispatchEvent(Event<object> eventNotification)
        {
            if (eventNotification is not null)
            {
                var configurations = await _webhookConfigurationRepository.GetWebhookConfigurationsAsync(eventNotification.Name.ToString());
                if (configurations != null && configurations.Any())
                {
                    foreach (var configuration in configurations)
                    {
                        var httpClient = new HttpClient();
                        if (!_policies.ContainsKey(configuration.Id.ToString()))
                        {
                            _policies.Add(configuration.Id.ToString(), Policies.CreatePolicy());
                        }
                        var _circuitBreaker = _policies[configuration.Id.ToString()] as AsyncCircuitBreakerPolicy;

                        var context = new Context();
                        try
                        {
                            var resultado = await _circuitBreaker.ExecuteAsync((context) =>
                            {
                                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, configuration.Uri);
                                httpRequestMessage.Content = JsonContent.Create(eventNotification);
                                return httpClient.SendAsync(httpRequestMessage);
                            }, context);
                            _logger.LogInformation($"* {DateTime.Now:HH:mm:ss} * " +
                            $"Circuito = {_circuitBreaker.CircuitState}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"# {DateTime.Now:HH:mm:ss} # " +
                            $"Circuito = {_circuitBreaker.CircuitState} | " +
                            $"Falha ao invocar a API: {ex.GetType().FullName} | {ex.Message}");
                        }
                    }
                }
            }
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

                    var eventNotification = JsonConvert.DeserializeObject<Event<object>>(result.Message.Value);

                    _ = DispatchEvent(eventNotification);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}