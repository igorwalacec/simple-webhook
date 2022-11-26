using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Webhook.Api.Repositories.Interfaces;

namespace Webhook.Api.Repositories;

public class WebhookConfigurationRepository : IWebhookConfigurationRepository
{
    private readonly IDistributedCache distributedCache;

    public WebhookConfigurationRepository(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    }

    public async Task<WebhookConfiguration> AddAsync(WebhookConfiguration configuration)
    {
        var configurationJson = JsonSerializer.Serialize(configuration);
        await distributedCache.SetStringAsync(configuration.Id.ToString(), configurationJson, CancellationToken.None);

        return configuration;
    }

    public async Task DeleteAsync(Guid id) => 
        await distributedCache.RemoveAsync(id.ToString());

    public async Task<WebhookConfiguration?> GetWebhookConfigurationAsync(Guid id)
    {
        var configurationJson = await distributedCache.GetStringAsync(id.ToString());
        if (string.IsNullOrEmpty(configurationJson))
            return null;
        return JsonSerializer.Deserialize<WebhookConfiguration>(configurationJson);
    }

    public async Task<WebhookConfiguration?> UpdateAsync(WebhookConfiguration configuration)
    {
        var oldConfigurationJson = await distributedCache.GetStringAsync(configuration.Id.ToString());
        if (string.IsNullOrEmpty(oldConfigurationJson))
            return null;
        var newConfigurationJson = JsonSerializer.Serialize(configuration);
        await distributedCache.SetStringAsync(configuration.Id.ToString(), newConfigurationJson, CancellationToken.None);

        return configuration;
    }
}
