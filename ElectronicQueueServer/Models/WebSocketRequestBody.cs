using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Models
{
    public class WebSocketRequestBody
    {
        public string Instraction { get; set; }
        public string Data { get; set; }
    }
}
