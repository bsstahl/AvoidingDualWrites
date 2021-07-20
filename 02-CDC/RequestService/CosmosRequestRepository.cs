using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace RequestService
{
    public class CosmosRequestRepository : IRequestRepository
    {
        // const string _insertRequestSql = "insert into tblRequests (Id, CustomerEmail, Description) values (@id, @customerEmail, @description)";

        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _config;

        public CosmosRequestRepository(IConfiguration config, CosmosClient cosmosClient)
        {
            _config = config;
            _cosmosClient = cosmosClient;
        }

        public void SaveRequest(Guid id, string customerEmail, string description)
        {
            Log.Information("Begin SaveRequest for {id}", id);

            // TODO: Retry multiple times if the process fails

            try
            {
                var requestData = new StorageRequest()
                {
                    Id = id,
                    CustomerEmail = customerEmail,
                    Description = description
                };

                var container = _cosmosClient.GetContainer(_config["DatabaseId"], _config["ContainerId"]);
                var createItemTask = container.CreateItemAsync<StorageRequest>(requestData, new PartitionKey(id.ToString()));
                Task.WaitAll(createItemTask);

                if (!createItemTask.IsCompletedSuccessfully)
                    throw new InvalidOperationException(createItemTask?.Exception?.Message ?? "Unable to create request in Cosmos DB");

                Log.Information("SaveRequest for {id} completed successfully", id);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to complete SaveRequest for {id} due to {error}", id, ex.Message);
                throw;
            }
        }
    }
}
