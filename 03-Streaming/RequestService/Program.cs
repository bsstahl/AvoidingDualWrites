using System;
using Confluent.Kafka;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RequestService
{
    class Program
    {
        const string _bootstrapServersKey = "BootstrapServers";
        const string _saslUsernameKey = "ClientKey";
        const string _saslPasswordKey = "ClientSecret";

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console().MinimumLevel.Verbose()
              .CreateLogger();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Engine>()
                .Build();

            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = config[_bootstrapServersKey],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = config[_saslUsernameKey],
                SaslPassword = config[_saslPasswordKey]
            };

            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddScoped<IIssueRequests, Engine>()
                .AddScoped<IRequestRepository, KafkaRequestRepository>()
                .AddScoped(c => producerConfig)
                .AddScoped(c => new ProducerBuilder<string, string>(c.GetRequiredService<ProducerConfig>()))
                .BuildServiceProvider();

            // TODO: Validate input

            var request = new Request()
            {
                Id = Guid.NewGuid(),
                CustomerEmail = args[0],
                Description = args[1]
            };

            var engine = services.GetRequiredService<IIssueRequests>();
            engine.IssueRequest(request);
        }
    }
}
