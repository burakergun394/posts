using RabbitMQ.Client;

namespace RabbitMQManagement.Connection;

public interface IRabbitMQConnectionFactory
{
    Task CreateConnectionAsync();
}

internal class RabbitMQConnectionFactory(RabbitMQConnectionOptions rabbitMQConnectionOptions) : IRabbitMQConnectionFactory
{
    private IAutorecoveringConnection? connection;

    public Task CreateConnectionAsync()
    {
        if (connection is not null)
        {
            return Task.FromResult(connection);
        }

        ConnectionFactory connectionFactory = GetConnectionFactory(rabbitMQConnectionOptions);

        connection = (IAutorecoveringConnection)connectionFactory.CreateConnection();
        return Task.CompletedTask;
    }

    private static ConnectionFactory GetConnectionFactory(RabbitMQConnectionOptions rabbitMQConnectionOptions)
    {
        return new ConnectionFactory
        {
            HostName = rabbitMQConnectionOptions.HostName,
            UserName = rabbitMQConnectionOptions.UserName,
            Password = rabbitMQConnectionOptions.Password,
            Port = rabbitMQConnectionOptions.Port,
            ClientProvidedName = rabbitMQConnectionOptions.ClientProvidedName
        };
    }
}

public class RabbitMQConnectionOptions
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string ClientProvidedName { get; set; } = $"RabbitMqManagement_{Environment.MachineName}_{new Random().Next(100)}";
}