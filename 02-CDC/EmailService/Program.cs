using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using EmailService.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EmailService
{
    class Program
    {
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
                            .AddUserSecrets<Engine>();
                    }
                )
                .ConfigureLogging((hostContext, logging) => logging.AddSerilog())
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetCosmosConnectionString();
                    var clientOptions = new CosmosClientOptions()
                    {
                        SerializerOptions = new CosmosSerializationOptions()
                        {
                            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                        }
                    };

                    _ = services
                        .AddSingleton<IConfiguration>(hostContext.Configuration)
                        .AddScoped<IHandleUnsentMessages, Engine>()
                        .AddScoped<ISendMessages, EmailClient.Engine>()
                        .AddScoped<CosmosClient>(c => new CosmosClient(connectionString, clientOptions))
                        .AddHostedService<CosmosChangeMonitor>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
