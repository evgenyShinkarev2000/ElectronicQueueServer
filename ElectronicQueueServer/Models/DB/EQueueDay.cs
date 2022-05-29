using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models.DB
{
    public class EQueueDay
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("records")]
        public IEnumerable<EQueueRecord> Records { get; set; }
    }
}
