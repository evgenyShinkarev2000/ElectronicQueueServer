using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;

namespace ElectronicQueueServer.Models.DB
{
    public class EQueueRecord
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("registeredId")]
        public string RegisteredId { get; set; }
        [JsonProperty("receiptTimeBegin")]

        public string ReceiptTimeBegin { get; set; }
        [JsonProperty("receiptTimeEnd")]
        public string ReceiptTimeEnd { get; set; }

        [JsonProperty("recordType")]
        public string RecordType { get; set; }

        [JsonProperty("info")]
        public object Info { get; set; }

        public static class RecordTypeName
        {
            public const string Reception = "reception";
            public const string Rest = "rest";
        }
    }
}
