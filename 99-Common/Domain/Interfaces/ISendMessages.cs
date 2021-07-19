using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISendMessages
    {
        void Send(string sendToName, string sendToAddress, string subject, string body);
    }
}
