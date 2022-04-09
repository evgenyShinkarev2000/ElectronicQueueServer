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
        private readonly List<WebSocket> _logUser = new List<WebSocket>();
        private readonly HashSet<WebSocket> _clients = new HashSet<WebSocket>();
        private readonly Dictionary<WebSocket, HashSet<ObjectId>> _clientLocksDict = new Dictionary<WebSocket, HashSet<ObjectId>>();
        private readonly HashSet<ObjectId> _lockedIds = new HashSet<ObjectId>();
        private readonly AppDB _appDB;
        private event Func<User, WebSocket, Task> _onUserCollectionChange;
        private event Func<LockedItem, WebSocket, Task> _onItemLockedChange;
        public WebSocketUserController(AppDB appDB)
        {
            this._appDB = appDB;
            _onItemLockedChange += this.UpdateLockedItems;
            _onUserCollectionChange += this.UpdateUserCollection;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task CreateWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await UserController(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task UserController(WebSocket webSocket)
        {
            _clients.Add(webSocket);
            _clientLocksDict.Add(webSocket, new HashSet<ObjectId>());
            _logUser.Add(webSocket);

            var buffer = new byte[1024 * 4];
            //await webSocket.SendAsync(Encoding.UTF8.GetBytes("Hello client!"), WebSocketMessageType.Text, true, CancellationToken.None);
            var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);


            while (!receiveResult.CloseStatus.HasValue)
            {
                var receiveString = Encoding.UTF8.GetString(buffer);
                var instruction = JsonConvert.DeserializeObject<WSMessageToServer>(receiveString).ServerInstructions;
                var response = await MakeResponse(instruction, receiveString, webSocket);
                if (response != null)
                {
                    await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(response),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
                var waiter = webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                receiveResult = await waiter;
            }

            _clients.Remove(webSocket);
            _clientLocksDict[webSocket].Select(lockedId => _lockedIds.Remove(lockedId));
            _clientLocksDict.Remove(webSocket);

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        private async Task<string> MakeResponse(IEnumerable<string> instruction, string receiveString, WebSocket webSocket)
        {
            object response = null;
            switch (instruction.First())
            {
                case WSMessageToServer.Instructions.GetAllUsers:
                    response = await GetUsers();
                    break;
                case WSMessageToServer.Instructions.GetEditRight:
                    var data = JsonConvert.DeserializeObject<WSMessageToServer>(receiveString).ServerData;
                    response = TryGetEditRight(new LockedItem(data), webSocket);
                    break;
                case WSMessageToServer.Instructions.DeleteEditRight:
                    var t = JsonConvert.DeserializeObject<WSMessageToServer>(receiveString).ServerData;
                    DeleteEditRight(new LockedItem(t).ItemId, webSocket);
                    
                    break;

            }

            return response == null ? null : JsonConvert.SerializeObject(response);
            
        }

        private void DeleteEditRight(ObjectId objectId, WebSocket webSocket)
        {
            this._lockedIds.Remove(objectId);
            this._clientLocksDict[webSocket].Remove(objectId);
            this._onItemLockedChange(new LockedItem() { ItemId = objectId, Status = LockedItem.LockedStatus.Free }, webSocket);
        }

        private WSMessageToClient TryGetEditRight(LockedItem item, WebSocket webSocket)
        {
            if (this._lockedIds.Contains(item.ItemId))
            {
                return new WSMessageToClient(WSMessageToClient.Instractions.EditRightResponse, false);
            }
            
            this._lockedIds.Add(item.ItemId);
            this._clientLocksDict[webSocket].Add(item.ItemId);
            this._onItemLockedChange(item, webSocket);

            return new WSMessageToClient(WSMessageToClient.Instractions.EditRightResponse, true);
        }


        private async Task<WSMessageToClient> GetUsers()
        {
            var usersCollection = (await _appDB.GetAllUsers()).Select(user =>
            {
                user.Status = this._lockedIds.Contains(user.Id) ? LockedItem.LockedStatus.Lock : LockedItem.LockedStatus.Free;
                return user;
            });


            return new WSMessageToClient(WSMessageToClient.Instractions.AllUsersResponse, usersCollection);
        }



        private async Task UpdateUserCollection(User user, WebSocket webSocket)
        {

        }

        private async Task UpdateLockedItems(LockedItem item, WebSocket webSocket)
        {
            
        }
    }
}