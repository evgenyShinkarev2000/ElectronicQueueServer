using MongoDB.Bson;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public IEnumerable<object> AccessRights { get; set; }
        public object Info { get; set; }
    }

    public static class UserRole
    {
        public const string Client = "Client";
        public const string Operator = "Operator";
        public const string Admin = "Admin";
    }
}
