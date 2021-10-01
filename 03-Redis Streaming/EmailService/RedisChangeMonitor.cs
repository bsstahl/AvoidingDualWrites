using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using EmailService.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace EmailService
{
    public class RedisChangeMonitor : BackgroundService
    {

        private readonly string _topicKey;
        private readonly string _consumerGroupName;
        private readonly string _clientId;
        private readonly int _emptyStreamDelay;

        private readonly IHandleUnsentMessages _messageHandler;
        private readonly ConnectionMultiplexer _redis;

        public string InstanceId { get; private set; }

        public RedisChangeMonitor(IConfiguration config, IHandleUnsentMessages messageHandler)
        {
            this.InstanceId = Guid.NewGuid().ToString();

            _topicKey = config["RedisStreamName"];
            _consumerGroupName = config["ConsumerGroupId"];
            _clientId = config["ClientId"];
            _emptyStreamDelay = Int32.Parse(config["EmptyStreamDelayMs"]);

            _messageHandler = messageHandler;
            _redis = ConnectionMultiplexer.Connect(config["RedisConnection"]);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting Redis Stream Consumer");
            var consumer = _redis.GetDatabase();
            await this.SubscribeAsync(consumer, stoppingToken);
            Log.Information("Stopping Redis Stream Consumer");
        }

        private async Task SubscribeAsync(IDatabase consumer, CancellationToken stoppingToken)
        {
            Log.Information("Subscribed to stream {topicName}", _topicKey);

            var streamInfo = consumer.StreamInfo(_topicKey);
            var jsonInfo = JsonConvert.SerializeObject(streamInfo);
            Log.Debug("Stream Content: {content}", jsonInfo);

            _ = consumer.GetConsumerGroup(_topicKey, _consumerGroupName, StreamPosition.NewMessages);

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await consumer.StreamReadGroupAsync(_topicKey, _consumerGroupName, _clientId, StreamPosition.NewMessages);
                {
                    if (!messages.Any())
                        await Task.Delay(_emptyStreamDelay, stoppingToken);
                    else
                    {
                        foreach (var message in messages)
                        {
                            await this.ProcessMessageAsync(message.Values[0].Name, message.Values[0].Value);
                        }
                    }
                }
            }

            Log.Debug("Cancellation Requested");
        }

        private async Task ProcessMessageAsync(string key, string value)
        {

            Log.Information("Processing message {key}", key);
            var request = JsonConvert.DeserializeObject<Request>(value);
            await _messageHandler.ProcessAsync(request);
            Log.Information("Processsing message {key} completed", value);
        }

    }
}
