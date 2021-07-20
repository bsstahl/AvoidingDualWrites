using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class Message
    {
        public Guid Id { get; set; }
        public string SendToAddress { get; set; }
        public string MessageSubject { get; set; }
        public string MessageBody { get; set; }
    }
}
