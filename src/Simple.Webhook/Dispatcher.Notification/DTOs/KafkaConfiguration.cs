namespace Dispatcher.Notification.DTOs;
public class KafkaConfiguration
{
    public string BootstrapServer { get; set; }
    public string TopicName { get; set; }
    public int MinPartition { get; set; }
}
