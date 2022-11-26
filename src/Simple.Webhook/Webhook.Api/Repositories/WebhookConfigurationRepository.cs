using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Simple.Webhook.Shared;
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

    public async Task<WebhookConfiguration?> AddAsync(WebhookConfiguration configuration)
    {
        var configurationJson = await distributedCache.GetStringAsync(configuration.EventName);
        if(string.IsNullOrEmpty(configurationJson))
        {
            var newEventList = new List<WebhookConfiguration>() { configuration };
            var eventListJson = JsonSerializer.Serialize(newEventList);
            await distributedCache.SetStringAsync(configuration.EventName, eventListJson, CancellationToken.None);

            return configuration;
        }
        else
        {
            var listConfigurations = JsonSerializer.Deserialize<List<WebhookConfiguration>>(configurationJson);
            var oldConfiguration = listConfigurations.FirstOrDefault(x => x.Name == configuration.Name);
        
            if (oldConfiguration is not null)
                return null;
            listConfigurations.Add(configuration);
            var eventListJson = JsonSerializer.Serialize(listConfigurations);
            await distributedCache.SetStringAsync(configuration.EventName, eventListJson, CancellationToken.None);
            return configuration;
        }
    }

    public async Task DeleteAsync(string eventName, Guid id)
    {
        var configurationJson = await distributedCache.GetStringAsync(eventName);
        if (!string.IsNullOrEmpty(configurationJson))
        {
            var listConfigurations = JsonSerializer.Deserialize<List<WebhookConfiguration>>(configurationJson);
            var configuration = listConfigurations?.Where(x => x.Id == id).FirstOrDefault();
            if(configuration is not null)
            {
                listConfigurations?.Remove(configuration);
                var eventListJson = JsonSerializer.Serialize(listConfigurations);
                await distributedCache.SetStringAsync(configuration.EventName, eventListJson, CancellationToken.None);
            }
        }
    }

    public async Task<WebhookConfiguration?> GetWebhookConfigurationAsync(string eventName, Guid id)
    {
        var configurationsJson = await distributedCache.GetStringAsync(eventName);
        if (string.IsNullOrEmpty(configurationsJson))
            return null;
        return JsonSerializer.Deserialize<List<WebhookConfiguration>>(configurationsJson)
            ?.FirstOrDefault(x => x.Id == id);
    }

    public async Task<WebhookConfiguration?> UpdateAsync(WebhookConfiguration configuration)
    {
        var configurationsJson = await distributedCache.GetStringAsync(configuration.EventName);
        if (string.IsNullOrEmpty(configurationsJson))
        {
            return null;
        }
        var listConfigurations = JsonSerializer.Deserialize<List<WebhookConfiguration>>(configurationsJson);
        var oldConfiguration = listConfigurations?.FirstOrDefault(x => x.Id == configuration.Id);
        if (oldConfiguration is not null)
        {
            listConfigurations.Remove(oldConfiguration);
            listConfigurations.Add(configuration);
        }
        var newConfigurationsJson = JsonSerializer.Serialize(listConfigurations);
        await distributedCache.SetStringAsync(configuration.EventName, newConfigurationsJson, CancellationToken.None);

        return configuration;
    }
}
