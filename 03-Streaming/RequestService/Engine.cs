using System;
using Domain.Entities;
using Domain.Interfaces;
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
            _repo.SaveRequest(request.Id, request.CustomerEmail, request.Description);
            Log.Information("Request created {id}", request.Id);
        }
    }
}
