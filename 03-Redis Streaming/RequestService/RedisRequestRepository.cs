using System;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Serilog;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RequestService
{
    public class RedisRequestRepository : IRequestRepository
    {

        private readonly string _streamName;
        private readonly ConnectionMultiplexer _redis;

        public RedisRequestRepository(IConfiguration config)
        {
            _redis = ConnectionMultiplexer.Connect(config["RedisConnection"]);
            _streamName = config["RedisStreamName"];
        }

        public void SaveRequest(Guid id, string customerEmail, string description)
        {
            Log.Information("Begin SaveRequest for {id}", id);

            // TODO: Retry multiple times if the process fails

            var key = id.ToString();
            var value = JsonConvert.SerializeObject(new Request()
            {
                Id = id,
                CustomerEmail = customerEmail,
                Description = description
            });

            try
            {
                // Produce message onto Redis stream
                var db = _redis.GetDatabase();
                var messageId = db.StreamAdd(_streamName, key, value);
                Log.Information("SaveRequest for {id} completed with Redis Id: {messageId}", key, messageId.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("Unable to complete SaveRequest for {id} due to {error}", id, ex.Message);
                throw;
            }
        }
    }
}
