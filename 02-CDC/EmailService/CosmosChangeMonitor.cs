using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using EmailService.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace EmailService
{
    public class CosmosChangeMonitor : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IHandleUnsentMessages _messageHandler;
        private readonly CosmosClient _client;

        private ChangeFeedProcessor _processor;

        public string InstanceId { get; private set; }

        public CosmosChangeMonitor(IConfiguration config, CosmosClient cosmosClient, IHandleUnsentMessages messageHandler)
        {
            this.InstanceId = Guid.NewGuid().ToString();
            _config = config;
            _client = cosmosClient;
            _messageHandler = messageHandler;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor = await _client.GetChangeFeedProcessor<object>(_config, this.ProcessMessageAsync, this.InstanceId, stoppingToken);
            await _processor.StartAsync();
        }

        internal async Task ProcessMessageAsync(IReadOnlyCollection<object> input, CancellationToken cancellationToken)
        {
            int i = 0;
            Log.Information($"Received message with {input.Count} document updates.");
            foreach (var item in input)
            {
                i++;
                Log.Information("Processing message {messageNumber} of {messageCount}", i, input.Count());
                var request = JsonConvert.DeserializeObject<Request>(item.ToString());
                await _messageHandler.ProcessAsync(request);
                Log.Information("Processsing message {messageNumber} completed", i);
            }
            Log.Information($"Message processing complete");
        }

    }
}
