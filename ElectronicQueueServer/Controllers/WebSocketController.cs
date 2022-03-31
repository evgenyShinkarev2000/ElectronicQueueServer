using ElectronicQueueServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Controllers
{
    [Route("[controller]")]
    public class WebSocketController : Controller
    {
        private static readonly HashSet<WebSocket> _clients = new HashSet<WebSocket>();
        private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        private readonly HashSet<ObjectId> _lockedId = new HashSet<ObjectId>();
        private readonly AppDB _appDB;
        private event Func<User, Task> _onUserCollectionChange;
        private event Func<BlockedItemData, Task> _onItemBlockedChange;
        public WebSocketController(AppDB appDB)
        {
            this._appDB = appDB;
            _onItemBlockedChange += this.UpdateBlockedItems;
            _onUserCollectionChange += this.UpdateUserCollection;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet, Route("users")]
        public async Task GetUsers()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await UserGetController(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task UserGetController(WebSocket webSocket)
        {
            _locker.EnterReadLock();
            _clients.Add(webSocket);
            _locker.ExitReadLock();
            var buffer = new byte[1024 * 4];
            await webSocket.SendAsync(Encoding.UTF8.GetBytes("Hello client!"), WebSocketMessageType.Text, true, CancellationToken.None);
            var usersCollection = await _appDB.GetAllUsers();
            var enumerator = usersCollection.GetEnumerator();



            var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);


            while (!receiveResult.CloseStatus.HasValue)
            {
                var count = int.Parse(Encoding.UTF8.GetString(buffer));
                for (var i = 0; i < count; i++)
                {
                    if (enumerator.MoveNext())
                    {
                        await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(enumerator.Current)),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                    }
                }

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

        }

        [HttpPost, Route("users")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            await _appDB.AddUser(user);
            await _onUserCollectionChange.Invoke(user);

            return Ok();
        }

        [HttpPut, Route("users")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            await _onItemBlockedChange.Invoke(new BlockedItemData() { Id = user.Id, Status = BlockedStatus.Blocked });
            // code
            Thread.Sleep(1000);
            await _onItemBlockedChange.Invoke(new BlockedItemData() { Id = user.Id, Status = BlockedStatus.Free });
            await _onUserCollectionChange.Invoke(user);

            return Ok();
        }

        [HttpDelete, Route("users")]
        public async Task<IActionResult> RemoveUser([FromBody] User user)
        {
            // code
            await _onUserCollectionChange.Invoke(user);
            return Ok();
        }

        private async Task UpdateUserCollection(User user)
        {
            foreach (var webSocket in _clients)
            {
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }

        private async Task UpdateBlockedItems(BlockedItemData item)
        {
            switch (item.Status)
            {
                case BlockedStatus.Blocked:
                    _lockedId.Add(item.Id);
                    foreach (var webSocket in _clients)
                    {
                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    break;
                case BlockedStatus.Free:
                    _lockedId.Remove(item.Id);
                    foreach (var webSocket in _clients)
                    {
                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    break;
                default:
                    throw new Exception("Неизвестное состояние заблокированного объекта");
            }
        }


        [HttpGet, Route("time")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

        }
        private static async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            await webSocket.SendAsync(Encoding.UTF8.GetBytes("Hello client!"), WebSocketMessageType.Text, true, CancellationToken.None);
            var timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += (s, e) => webSocket.SendAsync(
                   Encoding.UTF8.GetBytes(DateTime.Now.ToString()),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                   Encoding.UTF8.GetBytes(DateTime.Now.ToString()),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }
}

