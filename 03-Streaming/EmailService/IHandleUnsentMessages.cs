using System;
using System.Threading.Tasks;
using Domain.Entities;

namespace EmailService
{
    public interface IHandleUnsentMessages
    {
        Task ProcessAsync(Request request);
    }
}
