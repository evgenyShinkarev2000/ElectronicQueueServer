using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Models
{
    // отправляется на фронт, чтобы изменить стили объекта
    // BsonId и BsonRepresentation нужно убрать
    public class LockedItem
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public LockedItem() { }

        public LockedItem(string itemId, string status)
        {
            this.ItemId = itemId; // написал ItemId, дебажил 30 минут
            this.Status = status;
        }

        public LockedItem GetCopyOtherStatus(string status)
        {
            return new LockedItem() { ItemId = this.ItemId, UserId = this.UserId, Status = status };
        }

        public static class LockedStatus
        {
            public const string Free = "Free";
            public const string Lock = "Lock";
        }
    }
}
