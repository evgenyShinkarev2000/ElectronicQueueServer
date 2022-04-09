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
        public LockedItem()
        {
        }

        public LockedItem(Dictionary<string, object> tokens)
        {
            foreach (var token in tokens)
            {
                var lower = token.Key.ToLower();

                if (token.Value == null && lower != "itemid")
                {
                    continue;
                }
                if (token.Value.GetType() != typeof(string))
                {
                    throw new ArgumentException($"по ключу {token.Key} не строка");
                }

                var value = (string)token.Value;

                
                switch (lower)
                {
                    case "itemid":
                        this.ItemId = ObjectId.Parse(value);
                        break;
                    case "userid":
                        this.UserId = ObjectId.Parse(value);
                        break;
                    case "status":
                        this.Status = value;
                        break;
                    default:
                        throw new ArgumentException("неизсвестный ключ " + token.Key);
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

        public static class LockedStatus
        {
            public const string Free = "Free";
            public const string Lock = "Lock";
        }
    }
}
