using Dispatcher.Notification.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Simple.Webhook.Shared;
using System.Text.Json;

namespace Dispatcher.Notification.Repositories;

public class WebhookConfigurationRepository : IWebhookConfigurationRepository
{
    private readonly IDistributedCache distributedCache;

    public WebhookConfigurationRepository(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    }

    public async Task<IEnumerable<WebhookConfiguration>?> GetWebhookConfigurationsAsync(string eventName)
    {
        var configurationsJson = await distributedCache.GetStringAsync(eventName);
        if (string.IsNullOrEmpty(configurationsJson))
            return null;
        return JsonSerializer.Deserialize<List<WebhookConfiguration>>(configurationsJson);
    }
}
