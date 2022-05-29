using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace ElectronicQueueServer.Models.DB
{
    public class EQRecordPattern
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("beginTime")]

        public string BeginTime { get; set; }

        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        

        public static class RecordTypeName
        {
            public const string Reception = "reception";
            public const string Rest = "rest";
        }
    }
}
