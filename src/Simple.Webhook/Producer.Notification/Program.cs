using Producer.Notification;
using Simple.Webhook.Shared.Infra.Kafka;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services)=>
    {
        services.AddOptions<KafkaConfiguration>().BindConfiguration("KafkaConfiguration");
        services.AddKafkaProducer(context.Configuration);
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
