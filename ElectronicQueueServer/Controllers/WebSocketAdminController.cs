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
    public class WebSocketAdminController : Controller
    {
        private readonly WSUserHandler _socketHandler;
        private WebSocket _webSocket;

        public WebSocketAdminController(WSUserHandler handler)
        {
            _socketHandler = handler;
        }

        [Authorize(Roles = "Admin")]
        [Route("Ticket")]
        public ActionResult<string> GetTicket()
        {
            try
            {
                return _socketHandler.TicketMenager.GenerateTicket();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task CreateWebSocket(string webSocketTicket)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    if (!_socketHandler.TicketMenager.isTicketValid(webSocketTicket))
                    {
                        await Response.WriteAsync(Unauthorized().ToString());
                    }
                    else
                    {
                        _webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                        await _socketHandler.OnConnection(_webSocket);
                        await UserController();
                    }
                }
                catch (Exception ex)
                {
                    await this._socketHandler.OnDisconnected(_webSocket);
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
                {WSMessageToServer.Instructions.DeleteEditRight, () => _socketHandler.DeleteEditRight(_webSocket, new LockedItem(data))},
                {WSMessageToServer.Instructions.AddUser, () => _socketHandler.AddUser(data.ToObject<User>()) },
                {WSMessageToServer.Instructions.UdpateUser, () => _socketHandler.UpdateUser(data.ToObject<User>()) },
                {WSMessageToServer.Instructions.DeleteUser, () => _socketHandler.DeleteUser(data.ToObject<User>()) }
            };

            var instruction = instructions.First();
            await handlers[instruction]();
        }
    }
}