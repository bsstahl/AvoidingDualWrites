using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace RequestService
{
    public class KafkaRequestRepository : IRequestRepository
    {

        private readonly ProducerBuilder<string, string> _producerBuilder;
        private readonly string _topicName;

        public KafkaRequestRepository(IConfiguration config, ProducerBuilder<string, string> producerBuilder)
        {
            _producerBuilder = producerBuilder;
            _topicName = config["KafkaTopicName"];
        }

        public void SaveRequest(Guid id, string customerEmail, string description)
        {
            Log.Information("Begin SaveRequest for {id}", id);

            // TODO: Retry multiple times if the process fails

            try
            {
                var message = new Message<string, string>
                {
                    Key = id.ToString(),
                    Value = JsonConvert.SerializeObject(new Request()
                        {
                            Id = id,
                            CustomerEmail = customerEmail,
                            Description = description
                        })
                };

                using (var producer = _producerBuilder.Build())
                {
                    producer.Produce(_topicName, message, this.DeliveryHandler);
                    producer.Flush();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to complete SaveRequest for {id} due to {error}", id, ex.Message);
                throw;
            }
        }

        private void DeliveryHandler(DeliveryReport<string, string> report)
        {
            if (report.Status == PersistenceStatus.Persisted)
            {
                Log.Information("SaveRequest for {id} completed successfully", report.Key);
                Log.Verbose("Headers: {headers}", report.Headers);
                Log.Verbose("Partition: {partition}", report.Partition.Value);
                Log.Verbose("Offset: {offset}", report.Offset.Value);
                Log.Verbose("Message: {message}", report.Value);
            }
            else
            {
                var errorMessage = report?.Error?.Reason ?? $"Unable to create request in Cosmos DB for {report.Key}";
                Log.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}
