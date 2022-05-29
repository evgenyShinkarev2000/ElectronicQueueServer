using Newtonsoft.Json;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models
{
    public class WSMessageToServer
    {
        [JsonProperty("serverInstructions")]
        public IEnumerable<string> ServerInstructions { get; set; }
        [JsonProperty("serverData")]
        public object ServerData { get; set; }

        public WSMessageToServer() { }
        public WSMessageToServer(string instruction)
        {
            this.ServerInstructions = new List<string>() { instruction };
        }

        public WSMessageToServer(List<string> instruction)
        {
            this.ServerInstructions = instruction;
        }

        public WSMessageToServer(string instruction, object data)
        {
            this.ServerInstructions = new List<string>() { instruction };
            this.ServerData = data;
        }

        public WSMessageToServer(List<string> instructions, object data)
        {
            this.ServerInstructions = instructions;
            this.ServerData = data;
        }

        public static class Instructions
        {
            public const string GetAllUsers = "getAllUsers";
            public const string GetEditRight = "getEditRight";
            public const string DeleteEditRight = "deleteEditRight";
            public const string UdpateUser = "updateUser";
            public const string DeleteUser = "deleteUser";
            public const string AddUser = "addUser";
        }
    }
}
