using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Domain.Entities;
using EmailService.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace EmailService
{
    public class KafkaChangeMonitor : BackgroundService
    {
        private readonly ConsumerBuilder<string, string> _consumerBuilder;
        private readonly IHandleUnsentMessages _messageHandler;
        private readonly string _topicName;

        public string InstanceId { get; private set; }

        public KafkaChangeMonitor(IConfiguration config, ConsumerBuilder<string, string> consumerBuilder, IHandleUnsentMessages messageHandler)
        {
            this.InstanceId = Guid.NewGuid().ToString();
            _topicName = config["KafkaTopicName"];
            _consumerBuilder = consumerBuilder;
            _messageHandler = messageHandler;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting Kafka Consumer");
            using (var consumer = _consumerBuilder.Build())
                await this.SubscribeAsync(consumer, stoppingToken);
            Log.Information("Stopping Kafka Consumer");
        }

        private async Task SubscribeAsync(IConsumer<string, string> consumer, CancellationToken stoppingToken)
        {
            consumer.Subscribe(_topicName);
            Log.Information("Subscribed to topic {topicName}", _topicName);

            ConsumeResult<string, string> result;
            do
            {
                try
                {
                    result = consumer.Consume(stoppingToken);
                    if (result.IsNotNull())
                        await this.ProcessMessageAsync(result);
                }
                catch (OperationCanceledException)
                {
                    result = null;
                }
            } while (result.IsNotNull());
        }

        private async Task ProcessMessageAsync(ConsumeResult<string, string> input)
        {
            Log.Information("Processing message {key}", input.Message.Key);
            var request = JsonConvert.DeserializeObject<Request>(input.Message.Value);
            await _messageHandler.ProcessAsync(request);
            Log.Information("Processsing message {key} completed", input.Message.Key);
        }

    }
}
