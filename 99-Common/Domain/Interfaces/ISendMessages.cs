using System;

namespace Domain.Interfaces
{
    public interface ISendMessages
    {
        void Send(string sendToName, string sendToAddress, string subject, string body);
    }
}
