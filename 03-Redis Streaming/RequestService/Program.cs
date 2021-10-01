using System;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RequestService
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console().MinimumLevel.Verbose()
              .CreateLogger();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Engine>()
                .Build();

            // TODO: Add Producer Services
            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddScoped<IIssueRequests, Engine>()
                .AddScoped<IRequestRepository, RedisRequestRepository>()
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
