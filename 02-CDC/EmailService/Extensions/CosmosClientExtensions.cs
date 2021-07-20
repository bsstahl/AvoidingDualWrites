using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace EmailService.Extensions
{
    public static class CosmosClientExtensions
    {
        public static async Task<ChangeFeedProcessor> GetChangeFeedProcessor<T>(this CosmosClient client, IConfiguration config, Container.ChangesHandler<T> onMessagesReceived, string instanceId, CancellationToken cancellationToken)
        {
            var configSection = config.GetSection("CosmosChangeFeedProcessor");
            var databaseId = configSection["DatabaseId"];
            var containerId = configSection["ContainerId"];
            var leaseContainerId = configSection["LeaseContainerId"];
            var leaseContainerThroughput = configSection.GetValue<int>("LeaseContainerThroughput");
            var changeProcessorName = configSection["ChangeProcessorName"];
            var changeFeedMaxItems = configSection.GetValue<int>("MaxItems");
            var pollIntervalInMs = configSection.GetValue<int>("PollIntervalInMs");
            var changeFeedInstanceName = $"{changeProcessorName}_{instanceId}";

            var database = client.GetDatabase(databaseId);
            var container = database.GetContainer(containerId);

            var leaseContainerRequestOptions = new RequestOptions();
            var leaseContainerProperties = new ContainerProperties(leaseContainerId, "/id");
            var leaseContainerResponse = await database
                .CreateContainerIfNotExistsAsync(
                    leaseContainerProperties,
                    leaseContainerThroughput,
                    leaseContainerRequestOptions,
                    cancellationToken);

            var leaseContainer = leaseContainerResponse.Container;

            Log.Debug($"Monitored Container: {container.Id}");
            Log.Debug($"Lease Container: {leaseContainer.Id}");
            Log.Debug($"Processor Name: {changeProcessorName}");
            Log.Debug($"Instance Id: {instanceId}");
            Log.Debug($"Instance Name: {changeFeedInstanceName}");
            Log.Debug($"Message Handler: {onMessagesReceived.Method.DeclaringType.Name}.{onMessagesReceived.Method.Name}");

            return container
                .GetChangeFeedProcessorBuilder<T>(changeProcessorName, onMessagesReceived)
                .WithInstanceName(changeFeedInstanceName)
                .WithLeaseContainer(leaseContainer)
                .WithMaxItems(changeFeedMaxItems)
                .WithPollInterval(TimeSpan.FromMilliseconds(pollIntervalInMs))
                // .WithStartTime(DateTime.UtcNow.AddDays(-2))
                .Build();
        }

    }
}
