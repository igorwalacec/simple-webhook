namespace Simple.Webhook.Shared.Infra.Kafka;
public class KafkaConfiguration
{
    public string BootstrapServer { get; set; }
    public string TopicName { get; set; }
    public int MinPartition { get; set; }
}
