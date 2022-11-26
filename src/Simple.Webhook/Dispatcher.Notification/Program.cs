using Dispatcher.Notification;
using Dispatcher.Notification.DTOs;
using WorkerService1;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<KafkaConfiguration>().BindConfiguration("KafkaConfiguration");
        services.AddAsyncInitializer<TopicInitializer>();
        services.AddHostedService<Worker>();
    })
    .Build();
await host.InitAsync();
await host.RunAsync();
