using ElectronicQueueServer.Models.DB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models
{
    public class EQueue
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("creatorId")]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CreatorId { get; set; }

        [JsonProperty("days")]
        public IEnumerable<EQueueRecord> Days { get; set; }
    }
}
