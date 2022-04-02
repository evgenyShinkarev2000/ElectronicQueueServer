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
        private static readonly HashSet<WebSocket> _clients = new HashSet<WebSocket>();
        private static readonly Dictionary<WebSocket, HashSet<ObjectId>> _clientLocksDict = new Dictionary<WebSocket, HashSet<ObjectId>>();
        private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        private readonly HashSet<ObjectId> _lockedIds = new HashSet<ObjectId>();
        private readonly AppDB _appDB;
        private event Func<User, WebSocket, Task> _onUserCollectionChange;
        private event Func<LockedItem, WebSocket, Task> _onItemLockedChange;
        public WebSocketUserController(AppDB appDB)
        {
            this._appDB = appDB;
            _onItemLockedChange += this.UpdateBlockedItems;
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
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await UserController(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task UserController(WebSocket webSocket)
        {
            _locker.EnterReadLock();
            _clients.Add(webSocket);
            _clientLocksDict.Add(webSocket, new HashSet<ObjectId>());
            _locker.ExitReadLock();

            var buffer = new byte[1024 * 4];
            //await webSocket.SendAsync(Encoding.UTF8.GetBytes("Hello client!"), WebSocketMessageType.Text, true, CancellationToken.None);
            var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);


            while (!receiveResult.CloseStatus.HasValue)
            {
                var receiveString = Encoding.UTF8.GetString(buffer);
                var instruction = JsonConvert.DeserializeObject<WebSocketInstraction>(receiveString).Instruction;
                var response = await MakeResponse(instruction, receiveString, webSocket);
                if (response != null)
                {
                    await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(response),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            _locker.EnterReadLock();
            _clients.Remove(webSocket);
            _clientLocksDict[webSocket].Select(lockedId => _lockedIds.Remove(lockedId));
            _clientLocksDict.Remove(webSocket);
            _locker.ExitReadLock();

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        private async Task<string> MakeResponse(string instruction, string receiveString, WebSocket webSocket)
        {
            object response = null;
            switch (instruction)
            {
                case WebSocketMessageInstruction.AllUsers:
                    response = await GetUsers();
                    break;
                case WebSocketMessageInstruction.ChangeLock or WebSocketMessageInstruction.EditRight:
                    var data = JsonConvert.DeserializeObject<WebSocketMessage<Dictionary<string, string>>>(receiveString).Data;
                    response = ChangeLock(new LockedItem(data), webSocket);
                    break;
            }

            return JsonConvert.SerializeObject(response);
            
        }

        private object ChangeLock(LockedItem item, WebSocket webSocket)
        {
            WebSocketMessage<LockedItem> message = null;
            _locker.EnterWriteLock();
            if (item.Status == LockedStatus.Free)
            {
                _lockedIds.Remove(item.UserId);
                _clientLocksDict[webSocket].Remove(item.UserId);
            }
            else if (item.Status == LockedStatus.Locked)
            {
                message = new WebSocketMessage<LockedItem>(WebSocketMessageInstruction.EditRight);
                if (_lockedIds.Contains(item.ItemId))
                {
                    message.Data = item;
                }
                else
                {
                    _lockedIds.Add(item.ItemId);
                    _clientLocksDict[webSocket].Add(item.ItemId);
                    message.Data = item.GetCopyOtherStatus(LockedStatus.Free);
                }
            }
            _locker.ExitWriteLock();
            //_onItemLockedChange.Invoke(item, webSocket);
            return message;
        }


        private async Task<object> GetUsers()
        {
            var usersCollection = (await _appDB.GetAllUsers()).Select(user =>
            {
                user.Status = this._lockedIds.Contains(user.Id) ? LockedStatus.Locked : LockedStatus.Free;
                return user;
            });


            return new WebSocketMessage<IEnumerable<User>>(WebSocketMessageInstruction.AllUsers, usersCollection);
        }



        private async Task UpdateUserCollection(User user, WebSocket webSocket)
        {
            foreach (var client in _clients)
            {
                await client.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }

        private async Task UpdateBlockedItems(LockedItem item, WebSocket webSocket)
        {
            switch (item.Status)
            {
                case LockedStatus.Locked:
                    foreach (var client in _clients.Where(c => c != webSocket))
                    {
                        await client.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    break;
                case LockedStatus.Free:
                    foreach (var client in _clients.Where(c => c != webSocket))
                    {
                        await client.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    break;
                default:
                    throw new Exception("Неизвестное состояние заблокированного объекта");
            }
        }
    }
}