using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Models
{
    // отправляется на фронт, чтобы изменить стили объекта
    public class LockedItem
    {
        public LockedItem() { 
        }

        public LockedItem(Dictionary<string, string> tokens)
        {
            foreach(var token in tokens)
            {
                var lower = token.Key.ToLower();
                switch (lower)
                {
                    case "itemid":
                        this.ItemId = ObjectId.Parse(token.Value);
                        break;
                    case "userid":
                        this.UserId = ObjectId.Parse(token.Value);
                        break;
                    case "status":
                        this.Status = token.Value;
                        break;
                }
            }
        }
        
        [JsonProperty("itemId")]
        public ObjectId ItemId { get; set; }
        [JsonProperty("userId")]
        public ObjectId UserId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }

        public LockedItem GetCopyOtherStatus(string status)
        {
            return new LockedItem() { ItemId = this.ItemId, UserId = this.UserId, Status = status };
        }
    }

    public static class LockedStatus
    {
        public const string Free = "Free";
        public const string Locked = "Lock";
    }
}
