using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

app.MapPost("/webhook", async([FromBody]WebhookConfigurationDTO configuration, [FromServices] IWebhookConfigurationRepository repository) =>
{
    var newConfiguration = new WebhookConfiguration(configuration.Uri, configuration.Name, configuration.EventName);
    var result = await repository.AddAsync(newConfiguration);

    return result is null ? Results.BadRequest() : Results.Created($"/webhook/{result.EventName}", result);
})
.WithName("Create Webhook")
.WithOpenApi();

app.MapPut("/webhook/{eventName}/{id}", async ([FromRoute] string eventName, [FromRoute] Guid id, [FromBody]WebhookConfigurationDTO configuration, [FromServices]IWebhookConfigurationRepository repository) =>
{
    var oldConfiguration = new WebhookConfiguration(id, configuration.Uri, configuration.Name, configuration.EventName);
    var newConfiguration = await repository.UpdateAsync(oldConfiguration);
    if (newConfiguration is null)
        return Results.NotFound();

    return Results.Accepted($"/webhook/{newConfiguration.Id}", newConfiguration);
})
.WithName("Update Webhook")
.WithOpenApi();

app.MapGet("/webhook/{eventName}/{id}", async ([FromRoute] string eventName, [FromRoute] Guid id, [FromServices] IWebhookConfigurationRepository repository) =>
{
    var configuration = await repository.GetWebhookConfigurationAsync(eventName, id);
    return configuration is null ? Results.NotFound() : Results.Ok(configuration);
})
.WithName("Get Webhook by Id")
.WithOpenApi();

app.MapDelete("/webhook/{eventName}/{id}", async ([FromRoute] string eventName, [FromRoute] Guid id, [FromServices] IWebhookConfigurationRepository repository) =>
{
    await repository.DeleteAsync(eventName, id);
    return Results.NoContent();
})
.WithName("Delete Webhook by Id")
.WithOpenApi();

app.Run();
