using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public interface IMessageRepository
    {
        Message GetUnsentMessage();
        void UpdateMessageSent(Guid id);
    }
}
