using ElectronicQueueServer.Handlers;
using ElectronicQueueServer.Handlers.Factory;
using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
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
    [Route("ws/WebSocketAdmin")]
    public class UserController : Controller
    {
        private readonly SocketHandler handler;
        private readonly IWSControllerFactory controllerFactory;
        private readonly TicketMenager _ticketMenager;
        private WebSocket _webSocket;

        public UserController(UserControllerContainer container, IWSControllerFactory controllerFactory, AppDB appDB)
        {
            this.controllerFactory = controllerFactory;
            this._ticketMenager = container.TicketMenager;
            this.handler = container.SocketHandler;
            this.controllerFactory.SocketHandler = handler;
            this.controllerFactory.AppDB = appDB;
        }

        [Authorize(Roles = "ADMIN")]
        [Route("Ticket")]
        public ActionResult<string> GetTicket()
        {
            try
            {
                return this._ticketMenager.GenerateTicket();
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
                    if (!this._ticketMenager.isTicketValid(webSocketTicket))
                    {
                        await Response.WriteAsync(Unauthorized().ToString());
                    }
                    else
                    {
                        _webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                        this.controllerFactory.Role = "ADMIN";
                        this.controllerFactory.WebSocket = _webSocket;
                        await handler.OnConnection(_webSocket);
                        await UserSocket();
                    }
                }
                catch (Exception ex)
                {
                    await this.handler.OnDisconnected(_webSocket);
                }

            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task UserSocket()
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

                await RunProtectedHandler(instructions, data);

                receiveResult = _webSocket.ReceiveAsync(buffer, CancellationToken.None).Result;
            }

            await handler.OnDisconnected(_webSocket);
        }

        private async Task RunProtectedHandler(IEnumerable<string> instructions, Dictionary<string, object> data)
        {
            var mainController = new ElectronicQueueServer.Handlers.WSUser.Main(this.controllerFactory);
            try
            {
                await mainController.Handle(instructions, data);
            }
            catch(Exception e)
            {
                await handler.OnDisconnected(_webSocket);
            }
        }
    }
}