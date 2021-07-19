using System;

namespace RequestService
{
    public interface IRequestRepository
    {
        void SaveRequest(Guid id, string customerEmail, string description, string emailSubject, string emailBody);
    }
}