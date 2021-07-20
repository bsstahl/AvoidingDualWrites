using System;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Extensions;
using Serilog;

namespace RequestService
{
    public class Engine : IIssueRequests
    {
        private readonly IRequestRepository _repo;

        public Engine(IRequestRepository repo)
        {
            _repo = repo;
        }

        public void IssueRequest(Request request)
        {
            Log.Information("Request received {id} : {email} : {description}", request.Id, request.CustomerEmail, request.Description);

            // The logic to create the email information could also
            // be put in the email service. It is placed here because
            // this is where it would be if we were doing a dual-write
            string requestId = request.Id.ToString("D");
            string emailSubject = request.GetEmailSubject();
            string emailBody = request.GetEmailBody();

            _repo.SaveRequest(request.Id, request.CustomerEmail, request.Description, emailSubject, emailBody);

            Log.Information("Request created {id}", requestId);
        }
    }
}
