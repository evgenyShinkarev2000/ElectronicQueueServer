using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models
{
    public class WSMessageToClient
    {
        [JsonProperty("clientInstructions")]
        public List<string> ClientInstructions { get; set; }
        [JsonProperty("clientData")]
        public object ClientData { get; set; }

        public WSMessageToClient() { }
        public WSMessageToClient(string instruction)
        {
            this.ClientInstructions = new List<string>() { instruction };
        }

        public WSMessageToClient(List<string> instruction)
        {
            this.ClientInstructions = instruction;
        }

        public WSMessageToClient(string instruction, object data)
        {
            this.ClientInstructions = new List<string>() { instruction };
            this.ClientData = data;
        }

        public WSMessageToClient(List<string> instructions, object data)
        {
            this.ClientInstructions = instructions;
            this.ClientData = data;
        }

        public WSMessageToClient(string[] instructions, object data)
        {
            this.ClientInstructions = new List<string>(instructions);
            this.ClientData = data;
        }

        public static class Instractions
        {
            public const string EditRightResponse = "editRightResponse";
            public const string AllUsersResponse = "allUsersResponse";
            public const string UpdateLock = "updateLock";
            public const string UpdateUser = "updateUser";
            public const string DeleteUser = "deleteUser";
            public const string AddUser = "addUser";
        }
    }
}
