using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces;
using Serilog;

namespace EmailService
{
    public class Engine : IHandleUnsentMessages
    {
        private readonly ISendMessages _emailClient;

        public Engine(ISendMessages emailClient)
        {
            _emailClient = emailClient;
        }

        public async Task ProcessAsync(Request request)
        {
            Log.Information("Processing message {id}", request.Id);
            var messageSubject = request.GetEmailSubject();
            var messageBody = request.GetEmailBody();
            await Task.Run(() => _emailClient.Send("VIP", request.CustomerEmail, messageSubject, messageBody));
            Log.Information("Message {id} processed successfully", request.Id);
        }
    }
}
