using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RequestService
{
    [JsonObject]
    public class StorageRequest
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("customerEmail")]
        public string CustomerEmail { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
