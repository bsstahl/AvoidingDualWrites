using System;
using Microsoft.Extensions.Configuration;

namespace RequestService.Extensions
{
    public static class ConfigurationExtensions
    {
        const string _connection = "AccountEndpoint=https://{0}.documents.azure.com:443/;AccountKey={1};";

        public static string GetCosmosConnectionString(this IConfiguration config)
            => string.Format(_connection, config["CosmosDBEndpointName"], config["CosmosDBAccountKey"]);

    }
}
