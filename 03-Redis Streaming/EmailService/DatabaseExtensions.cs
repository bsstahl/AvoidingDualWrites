using System;
using Serilog;
using StackExchange.Redis;

namespace EmailService
{
    public static class DatabaseExtensions
    {
        public static bool GetConsumerGroup(this IDatabase client, string key, string groupName, RedisValue? position)
        {
            Log.Debug("Attempting to load or create consumer group '{groupName}' for stream '{streamKey}'", groupName, key);

            var createGroupResult = false;
            try
            {
                createGroupResult = client.StreamCreateConsumerGroup(key, groupName, position);
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to create consumer group due to error '{error}'", ex.Message);
            }   

            Log.Debug("Result of StreamCreateConsumerGroupAsync is {createGroupResult}", createGroupResult);

            return createGroupResult;
        }
    }
}
