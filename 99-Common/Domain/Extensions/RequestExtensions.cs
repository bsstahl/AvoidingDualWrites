using System;
using Domain.Entities;

namespace Domain.Extensions
{
    public static class RequestExtensions
    {
        public static string GetEmailSubject(this Request request)
            => $"(Do not reply) Request Received ({request.Id})";

        public static string GetEmailBody(this Request request)
            => $"<p>Your request with tracking Id <b>{request.Id}</b> was received and will be processed as soon as possible.</p>";
    }
}
