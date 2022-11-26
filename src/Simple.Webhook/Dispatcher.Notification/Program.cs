using Dispatcher.Notification;
using Dispatcher.Notification.Repositories;
using Dispatcher.Notification.Repositories.Interfaces;
using Dispatcher.Notification.Resilience;
using Polly.Registry;
using Simple.Webhook.Shared.Infra.Kafka;
using Simple.Webhook.Shared.Infra.Redis;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddRedis(context.Configuration);
        services.AddKafkaConsumer(context.Configuration);
        services.AddOptions<KafkaConfiguration>().BindConfiguration("KafkaConfiguration");
        services.AddSingleton<IWebhookConfigurationRepository, WebhookConfigurationRepository>();        
        services.AddSingleton<IPolicyRegistry<string>, PolicyRegistry>(provider =>
        {
            var registry = new PolicyRegistry
            {
                { "default", Policies.DefaultPolicy() }
            };

            return registry;
        });
        services.AddAsyncInitializer<TopicInitializer>();
        services.AddHostedService<Dispatcher.Notification.Dispatcher>();
    })
    .Build();
await host.InitAsync();
await host.RunAsync();
