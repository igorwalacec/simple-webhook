using System.Text.Json.Serialization;

public class WebhookConfiguration
{
    public WebhookConfiguration(string uri, string name, string eventName)
    {
        Id = Guid.NewGuid();
        Uri = uri;
        Name = name;
        EventName = eventName;
    }
    [JsonConstructor]
    public WebhookConfiguration(Guid id, string uri, string name, string eventName)
    {
        Id = id;
        Uri = uri;
        Name = name;
        EventName = eventName;
    }

    public Guid Id { get; set; }
    public string Uri { get; private set; }
    public string Name { get; private set; }
    public string EventName { get; private set; }
}
