using Dispatcher.Notification;
using Simple.Webhook.Shared.Infra.Kafka;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddKafkaConsumer(context.Configuration);
        services.AddOptions<KafkaConfiguration>().BindConfiguration("KafkaConfiguration");

        services.AddAsyncInitializer<TopicInitializer>();
        services.AddHostedService<Dispatcher.Notification.Dispatcher>();
    })
    .Build();
await host.InitAsync();
await host.RunAsync();
