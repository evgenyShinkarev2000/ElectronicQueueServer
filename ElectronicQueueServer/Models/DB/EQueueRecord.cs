using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace ElectronicQueueServer.Models.DB
{
    public class EQueueRecord
    {
        [JsonProperty("id")]
        public ObjectId Id { get; set; }
        [JsonProperty("registeredId")]
        public ObjectId RegisteredId { get; set; }
        [JsonProperty("receiptTimeBegin")]
        public TimeOnly ReceiptTimeBegin { get; set; }
        [JsonProperty("receiptTimeEnd")]
        public TimeOnly ReceiptTimeEnd { get; set; }

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
