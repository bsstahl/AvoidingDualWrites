using System;
using System.Diagnostics;
using System.Threading;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RequestService.Extensions;
using Serilog;

namespace RequestService
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console().MinimumLevel.Debug()
              .CreateLogger();

            var config = new ConfigurationBuilder()
                .AddUserSecrets<Engine>()
                .Build();

            var connectionString = config.GetCosmosConnectionString();
            var clientOptions = new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };

            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddScoped<IIssueRequests, Engine>()
                .AddScoped<IRequestRepository, CosmosRequestRepository>()
                .AddScoped<CosmosClient>(s => new CosmosClient(connectionString, clientOptions))
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
