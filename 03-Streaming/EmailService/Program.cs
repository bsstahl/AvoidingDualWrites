using System;
using System.IO;
using System.Threading.Tasks;
using Confluent.Kafka;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EmailService
{
    class Program
    {
        const string _bootstrapServersKey = "BootstrapServers";
        const string _saslUsernameKey = "ClientKey";
        const string _saslPasswordKey = "ClientSecret";
        const string _clientIdKey = "ClientId";

        async static Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console().MinimumLevel.Debug()
              .CreateLogger();

            var host = new HostBuilder()
                .ConfigureAppConfiguration(
                    builder =>
                    {
                        _ = builder
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", false)
                            .AddUserSecrets<Engine>()
                            .AddEnvironmentVariables();
                    }
                )
                .ConfigureLogging((hostContext, logging) => logging.AddSerilog())
                .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;

                    var consumerConfig = new ConsumerConfig()
                    {
                        GroupId = config["ConsumerGroupId"],
                        BootstrapServers = config[_bootstrapServersKey],
                        SecurityProtocol = SecurityProtocol.SaslSsl,
                        SaslMechanism = SaslMechanism.Plain,
                        SaslUsername = config[_saslUsernameKey],
                        SaslPassword = config[_saslPasswordKey],
                        ClientId = config[_clientIdKey]
                    };

                    _ = services
                        .AddSingleton<IConfiguration>(hostContext.Configuration)
                        .AddScoped<IHandleUnsentMessages, Engine>()
                        .AddScoped<ISendMessages, EmailClient.Engine>()
                        .AddScoped(c => consumerConfig)
                        .AddScoped(c => new ConsumerBuilder<string, string>(c.GetRequiredService<ConsumerConfig>()))
                        .AddHostedService<KafkaChangeMonitor>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
