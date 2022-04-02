using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Models
{
    public class WebSocketInstraction
    {
        [JsonProperty("instruction")]
        public string Instruction { get; set; }
    }
    public class WebSocketMessage<T>: WebSocketInstraction
    {
        public WebSocketMessage()
        {

        }

        public WebSocketMessage(string instruction)
        {
            this.Instruction = instruction;

        }
        public WebSocketMessage(string instruction, T data)
        {
            this.Instruction = instruction;
            this.Data = data;
        }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public static class WebSocketMessageInstruction
    {
        public const string AllUsers = "allUsers";
        public const string ChangeLock = "changeLock";
        public const string EditRight = "editRight";
    }
}
