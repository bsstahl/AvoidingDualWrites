using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using Serilog;

namespace EmailService
{
    public class Engine: IHandleUnsentMessages
    {
        // TODO: Make the message handler common for all demos

        const int _delayBetweenChecksMs = 1000;

        private readonly IMessageRepository _repo;
        private readonly ISendMessages _emailClient;

        public Engine(IMessageRepository repo, ISendMessages emailClient)
        {
            _repo = repo;
            _emailClient = emailClient;
        }

        public async Task ProcessAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting message processing");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_delayBetweenChecksMs); // Avoid a tight loop
                var message = _repo.GetUnsentMessage(); // Returns null if no messages are queued
                if (message is null)
                {
                    Log.Debug("No messages queued to be sent");
                }
                else
                {
                    Log.Information("Processing message {id}", message.Id);
                    _emailClient.Send("VIP", message.SendToAddress, message.MessageSubject, message.MessageBody);
                    _repo.UpdateMessageSent(message.Id);
                    Log.Information("Message {id} processed successfully", message.Id);
                }
            }

            Log.Information("Message processing ending");
        }
    }
}
