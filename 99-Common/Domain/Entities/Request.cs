using System;

namespace Domain.Entities
{
    public class Request
    {
        public Guid Id { get; set; }
        public string CustomerEmail { get; set; }
        public string Description { get; set; }
    }
}
