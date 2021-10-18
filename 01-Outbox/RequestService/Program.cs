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
              .WriteTo.Console().MinimumLevel.Debug()
              .CreateLogger();

            var config = new ConfigurationBuilder()
                .AddUserSecrets<Engine>()
                .AddJsonFile("settings.json")
                .Build();

            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddScoped<IIssueRequests, Engine>()
                .AddScoped<IRequestRepository, RequestRepository>()
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
