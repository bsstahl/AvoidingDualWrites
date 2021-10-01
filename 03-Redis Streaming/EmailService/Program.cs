using System;
using System.IO;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EmailService
{
    class Program
    {
        async static Task Main(string[] _1)
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

                    // TODO: Add proper services
                    _ = services
                        .AddSingleton<IConfiguration>(hostContext.Configuration)
                        .AddScoped<IHandleUnsentMessages, Engine>()
                        .AddScoped<ISendMessages, EmailClient.Engine>()
                        .AddHostedService<RedisChangeMonitor>();
                })
                .Build();

             await host.RunAsync();
        }
    }
}
