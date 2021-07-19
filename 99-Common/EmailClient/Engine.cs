using System;
using System.Reflection.Metadata.Ecma335;
using Domain.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Serilog;

namespace EmailClient
{
    public class Engine: ISendMessages
    {
        private readonly string _fromName;
        private readonly string _fromAddress;

        private readonly string _bccName;
        private readonly string _bccAddress;

        private readonly string _serverUsername;
        private readonly string _serverPassword;
        private readonly string _serverName;
        private readonly int _serverPort;

        public Engine(IConfiguration config)
        {
            _fromName = config["EmailFromName"];
            _fromAddress = config["EmailFromAddress"];
            _bccName = config["EmailBccName"];
            _bccAddress = config["EmailBccAddress"];
            _serverUsername = config["EmailUsername"];
            _serverPassword = config["EmailPassword"];
            _serverName = config["EmailServerName"];
            _serverPort = int.Parse(config["EmailServerPort"]);
        }

        public void Send(string sendToName, string sendToAddress, string subject, string body)
        {
            Log.Information("Sending email message with subject {subject}", subject);

            try
            {
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(_fromName, _fromAddress));
                msg.To.Add(new MailboxAddress(sendToName, sendToAddress));
                msg.Bcc.Add(new MailboxAddress(_bccName, _bccAddress));
                msg.Subject = subject;
                msg.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(_serverName, _serverPort, MailKit.Security.SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_serverUsername, _serverPassword);
                    client.Send(msg);
                }

                Log.Information("Email message with subject {subject} sent successfully", subject);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to send email message with subject {subject} due to {error}", subject, ex.Message);
                throw;
            }   
        }
    }
}
