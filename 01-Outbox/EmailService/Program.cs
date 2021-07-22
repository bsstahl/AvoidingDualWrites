using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EmailService
{
    class Program
    {
        async static Task Main(string[] args)
        {
            const int maxExecutionTimeMinutes = 10;

            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console().MinimumLevel.Debug()
              .CreateLogger();

            var config = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .AddUserSecrets<Engine>()
                .Build();

            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddScoped<IHandleUnsentMessages, Engine>()
                .AddScoped<IMessageRepository, MessageRepository>()
                .AddScoped<ISendMessages, EmailClient.Engine>()
                .BuildServiceProvider();

            var engine = services.GetRequiredService<IHandleUnsentMessages>();
            var tokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(maxExecutionTimeMinutes));
            await engine.ProcessAsync(tokenSource.Token);
        }
    }
}
