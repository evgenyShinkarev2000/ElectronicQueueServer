using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models.DB
{
    public class EQDay
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("creatorId")]
        public string CreatorId { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("records")]
        public IEnumerable<EQRecord> Records { get; set; }

        [JsonProperty("info")]
        public object Info { get; set; }
    }
}
