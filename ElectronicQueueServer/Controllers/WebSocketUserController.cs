using ElectronicQueueServer.Handlers;
using ElectronicQueueServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Controllers
{
    [Route("ws/[controller]")]
    public class WebSocketUserController : Controller
    {
        private readonly WebSocketUserHandler _socketHandler;
        private WebSocket _webSocket;

        public WebSocketUserController(WebSocketUserHandler handler)
        {
            _socketHandler = handler;
        }

        [HttpGet]
        public async Task CreateWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    _webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                    await _socketHandler.OnConnection(_webSocket);
                    await UserController();
                }
                catch (Exception ex)
                {
                    var a = "a";
                }

            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task UserController()
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await _webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                var receiveString = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                var messageToServer = JsonConvert.DeserializeObject<WSMessageToServer>(receiveString);
                var instructions = messageToServer.ServerInstructions;
                var data = messageToServer.ServerData;

                await RunHandler(instructions, data);
                
                receiveResult = _webSocket.ReceiveAsync(buffer, CancellationToken.None).Result;
            }

            await _socketHandler.OnDisconnected(_webSocket);
        }

        private async Task RunHandler(IEnumerable<string> instructions, Dictionary<string, object> data)
        {
            var handlers = new Dictionary<string, Func<Task>>()
            {
                {WSMessageToServer.Instructions.GetAllUsers, () => _socketHandler.GetUsers(_webSocket) },
                {WSMessageToServer.Instructions.GetEditRight, () => _socketHandler.GetEditRight(_webSocket, new LockedItem(data)) },
                {WSMessageToServer.Instructions.DeleteEditRight, () => _socketHandler.DeleteEditRight(_webSocket, new LockedItem(data))}
            };

            var instruction = instructions.First();
            await handlers[instruction]();
        }
    }
}