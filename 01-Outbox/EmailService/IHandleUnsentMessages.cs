using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmailService
{
    public interface IHandleUnsentMessages
    {
        Task ProcessAsync(CancellationToken stoppingToken);
    }
}
