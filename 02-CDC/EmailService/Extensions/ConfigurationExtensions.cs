using System;
using Microsoft.Extensions.Configuration;

namespace EmailService.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetCosmosConnectionString(this IConfiguration config)
            => string.Format(config.GetConnectionString("CosmosDb"), config["CosmosDBEndpointName"], config["CosmosDBAccountKey"]);
    }
}
