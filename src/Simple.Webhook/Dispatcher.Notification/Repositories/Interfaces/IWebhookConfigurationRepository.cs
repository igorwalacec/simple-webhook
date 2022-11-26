using Simple.Webhook.Shared;

namespace Dispatcher.Notification.Repositories.Interfaces;
public interface IWebhookConfigurationRepository
{
    Task<IEnumerable<WebhookConfiguration>?> GetWebhookConfigurationsAsync(string eventName);
}
