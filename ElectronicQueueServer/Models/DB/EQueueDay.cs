using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models.DB
{
    public class EQueueDay
    {
        [JsonProperty("id")]
        public ObjectId Id { get; set; }
        [JsonProperty("date")]
        public DateOnly Date { get; set; }
        [JsonProperty("records")]
        public IEnumerable<EQueueRecord> Records { get; set; }
    }
}
