using Simple.Webhook.Shared;

namespace Webhook.Api.Repositories.Interfaces;
public interface IWebhookConfigurationRepository
{
    Task<WebhookConfiguration?> GetWebhookConfigurationAsync(string eventName, Guid id);
    Task<WebhookConfiguration?> AddAsync(WebhookConfiguration configuration);
    Task<WebhookConfiguration?> UpdateAsync(WebhookConfiguration configuration);
    Task DeleteAsync(string eventName, Guid id);
}
