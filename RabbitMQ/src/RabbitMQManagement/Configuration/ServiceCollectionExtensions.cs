using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQManagement.Client;
using RabbitMQManagement.Connection;

namespace RabbitMQManagement.Configuration;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddRabbitMQManagement(this IServiceCollection services, IConfiguration configuration, string clientName)
    {
        return AddRabbitMQManagement(services, configuration, clientName, null);
    }

    public static IServiceCollection AddRabbitMQManagement(this IServiceCollection services, IConfiguration configuration, string clientName, Action<RabbitMQManagementOptions> options)
    {
        if (string.IsNullOrWhiteSpace(clientName))
            throw new Exception("Client name cannot be null or empty");

        var configurationSectionName = $"{RabbitMQManagementOptions.ConfigurationKey}:{clientName}";
        var rabbitMQManagementOptions  = new RabbitMQManagementOptions();
        options?.Invoke(rabbitMQManagementOptions);
        services.Configure<RabbitMQManagementOptions>(clientName.ToLowerInvariant(), configuration.GetSection(configurationSectionName));
        if (options is not null)
            services.Configure<RabbitMQManagementOptions>(clientName.ToLowerInvariant(), options);

        services.AddScoped<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();
        services.AddScoped<IRabbitMQClient, RabbitMQClient>();
        services.AddScoped<IRabbitMqClientName, RabbitMqClientName>();
        services.AddSingleton<IRabbitMQClientFactory, RabbitMQClientFactory>();

        return services;
    }
}

public class RabbitMQManagementOptions
{
    internal static string ConfigurationKey { get; set; } = "RabbitMQ";
    public RabbitMQConnectionOptions Connection { get; set; } = new RabbitMQConnectionOptions();
}
