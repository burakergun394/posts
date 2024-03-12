using Microsoft.Extensions.DependencyInjection;
using RabbitMQManagement.Connection;

namespace RabbitMQManagement.Client;

public interface IRabbitMQClient
{
    IRabbitMQConnectionFactory ConnectionFactory { get; }
}

internal class RabbitMQClient(IRabbitMQConnectionFactory rabbitMQConnectionFactory) : IRabbitMQClient
{
    public IRabbitMQConnectionFactory ConnectionFactory => rabbitMQConnectionFactory;
}

public interface IRabbitMQClientFactory
{
    IRabbitMQClient GetClient(string clientName);
}

internal class RabbitMQClientFactory(IServiceProvider serviceProvider): IRabbitMQClientFactory
{
    private readonly Dictionary<string, IRabbitMQClient> clients = [];
    private readonly object syncRoot = new();

    public IRabbitMQClient GetClient(string name)
    {

        var lowercaseName = name.ToLowerInvariant();

        if (clients.TryGetValue(lowercaseName, out var client))
        {
            return client;
        }

        lock (syncRoot)
        {
            using var scope = serviceProvider.CreateScope();
            var rabbitMqClient = scope.ServiceProvider.GetRequiredService<IRabbitMQClient>();
            var rabbitMqClientName = scope.ServiceProvider.GetRequiredService<IRabbitMqClientName>();
            rabbitMqClientName.Value = lowercaseName;
            clients[lowercaseName] = rabbitMqClient;
            return rabbitMqClient;
        }
    }
}

internal interface IRabbitMqClientName
{
    string Value { get; set; }
}

internal class RabbitMqClientName : IRabbitMqClientName
{
    public string Value { get; set; }
}