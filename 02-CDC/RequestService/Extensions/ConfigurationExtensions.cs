using System;
using Microsoft.Extensions.Configuration;

namespace RequestService.Extensions
{
    public static class ConfigurationExtensions
    {
        const string _cosmosConnectionKey = "Cosmos";
        const string _cosmosAccountKey = "CosmosDBAccountKey";
        const string _cosmosEndpointNameKey = "CosmosDBEndpointName";

        public static string GetCosmosConnectionString(this IConfiguration config)
            => string.Format(config.GetConnectionString(_cosmosConnectionKey), config[_cosmosEndpointNameKey], config[_cosmosAccountKey]);
    }
}
