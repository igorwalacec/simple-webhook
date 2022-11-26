namespace Webhook.Api.DTOs
{
    public class WebhookConfigurationDTO
    {
        public string Uri { get; set; }
        public string Name { get; set; }
        public string EventName { get; set; }
    }
}
