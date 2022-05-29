using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;

namespace ElectronicQueueServer.Models.DB
{
    public class EQRecord
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("registeredId")]
        public string RegisteredId { get; set; }

        [JsonProperty("beginTime")]

        public string BeginTime { get; set; }

        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("info")]
        public object Info { get; set; }


        public static class RecordTypeName
        {
            public const string Reception = "reception";
            public const string Rest = "rest";
        }
    }
}
