using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models
{
    public class User
    {
        [JsonProperty("id")]
        public ObjectId Id { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("secondName")]
        public string SecondName { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("accessRights")]
        public IEnumerable<object> AccessRights { get; set; }
        [JsonProperty("info")]
        public object Info { get; set; }
        [BsonIgnore, JsonProperty("status")]
        public string Status { get; set; }
    }

    public static class UserRole
    {
        public const string Client = "Client";
        public const string Operator = "Operator";
        public const string Admin = "Admin";
    }
}
