using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQManagement.Client;
using RabbitMQManagement.Configuration;

var host = CreateHostBuilder(args).Build();
var clientFactory = host.Services.GetRequiredService<IRabbitMQClientFactory>();

var client1 = clientFactory.GetClient("client1");
await client1.ConnectionFactory.CreateConnectionAsync();

var client2 = clientFactory.GetClient("client2");
await client2.ConnectionFactory.CreateConnectionAsync();

var client1Again = clientFactory.GetClient("client1");
await client1Again.ConnectionFactory.CreateConnectionAsync();

Console.ReadLine();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(Directory.GetCurrentDirectory());
        })
        .ConfigureServices((context, services) =>
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddEnvironmentVariables();
            var configurationRoot = builder.Build();
            services.AddRabbitMQManagement(configurationRoot, "Client1", (options) =>
            {
                options.Connection.HostName = "localhost";
                options.Connection.UserName = "user";
                options.Connection.Password = "password";
                options.Connection.Port = 5672;
            });
            services.AddRabbitMQManagement(configurationRoot, "Client2");
        });