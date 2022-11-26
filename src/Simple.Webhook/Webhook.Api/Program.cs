using Microsoft.AspNetCore.Http.HttpResults;
using Simple.Webhook.Shared;
using Simple.Webhook.Shared.Infra.Redis;
using Webhook.Api.DTOs;
using Webhook.Api.Repositories;
using Webhook.Api.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRedis(builder.Configuration);

builder.Services.AddSingleton<IWebhookConfigurationRepository, WebhookConfigurationRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/webhook", async(WebhookConfigurationDTO configuration, IWebhookConfigurationRepository repository) =>
{
    var newConfiguration = new WebhookConfiguration(configuration.Uri, configuration.Name, configuration.EventName);
    var result = await repository.AddAsync(newConfiguration);

    return Results.Created($"/webhook/{result.Id}", result);
})
.WithName("Create Webhook")
.WithOpenApi();

app.MapPut("/webhook/{id}", async (Guid id, WebhookConfigurationDTO configuration, IWebhookConfigurationRepository repository) =>
{
    var oldConfiguration = new WebhookConfiguration(id, configuration.Uri, configuration.Name, configuration.EventName);
    var newConfiguration = await repository.UpdateAsync(oldConfiguration);
    if (newConfiguration is null)
        return Results.NotFound();

    return Results.Accepted($"/webhook/{newConfiguration.Id}", newConfiguration);
})
.WithName("Update Webhook")
.WithOpenApi();

app.MapGet("/webhook/{id}", async (Guid id, IWebhookConfigurationRepository repository) =>
{
    var configuration = await repository.GetWebhookConfigurationAsync(id);
    return configuration is null ? Results.NotFound() : Results.Ok(configuration);
})
.WithName("Get Webhook by Id")
.WithOpenApi();

app.MapDelete("/webhook/{id}", async (Guid id, IWebhookConfigurationRepository repository) =>
{
    await repository.DeleteAsync(id);
    return Results.NoContent();
})
.WithName("Delete Webhook by Id")
.WithOpenApi();

app.Run();
