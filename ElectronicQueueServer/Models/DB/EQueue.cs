using ElectronicQueueServer.Models.DB;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models
{
    public class EQueue
    {
        [JsonProperty("id")]
        public ObjectId Id { get; set; }
        [JsonProperty("creatorId")]
        public ObjectId CreatorId { get; set; }
        [JsonProperty("days")]
        public IEnumerable<EQueueRecord> Days { get; set; }
    }
}
