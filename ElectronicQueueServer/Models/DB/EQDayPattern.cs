using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models.DB
{
    public class EQDayPattern
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("creatorId")]
        public string CreatorId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("records")]
        public IEnumerable<EQRecordPattern> Records { get; set; }

        [JsonProperty("info")]
        public object Info { get; set; }
    }
}
