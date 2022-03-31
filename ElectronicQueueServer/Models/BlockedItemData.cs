using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Models
{
    // отправляется на фронт, чтобы изменить стили объекта
    public class BlockedItemData
    {
        public ObjectId Id { get; set; }
        public string Status { get; set; }
    }

    public static class BlockedStatus
    {
        public const string Free = "Free";
        public const string Blocked = "Blocked";
    }
}
