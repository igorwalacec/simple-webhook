namespace Webhook.Api.Repositories.Interfaces;
public interface IWebhookConfigurationRepository
{
    Task<WebhookConfiguration?> GetWebhookConfigurationAsync(Guid id);
    Task<WebhookConfiguration> AddAsync(WebhookConfiguration configuration);
    Task<WebhookConfiguration?> UpdateAsync(WebhookConfiguration configuration);
    Task DeleteAsync(Guid id);
}
