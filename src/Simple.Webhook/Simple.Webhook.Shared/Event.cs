namespace Simple.Webhook.Shared;

public class Event<T>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public T Data { get; set; }
}
