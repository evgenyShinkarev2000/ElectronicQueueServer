using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers
{
    public class WebSocketUserHandler : SocketHandler
    {
        private readonly AppDB _appDB;
        private readonly ConcurrentDictionary<string, HashSet<string>> _userPairLocked = new ConcurrentDictionary<string, HashSet<string>>();
        public WebSocketUserHandler(ConnectionManager connectionManager, AppDB appDB) : base(connectionManager)
        {
            _appDB = appDB;
        }

        public override async Task OnConnection(WebSocket socket)
        {
            await base.OnConnection(socket);
            _userPairLocked.TryAdd(this.ConnectionsManager.GetId(socket), new HashSet<string>());
        }

        public override async Task OnDisconnected(WebSocket webSocket)
        {
            var id = this.ConnectionsManager.GetId(webSocket);
            _userPairLocked.TryRemove(id, out var lockedIdSet);
            foreach (var lockedId in lockedIdSet)
            {
                await this.UpdateLock(webSocket, new LockedItem()
                {
                    ItemId = MongoDB.Bson.ObjectId.Parse(lockedId),
                    Status = LockedItem.LockedStatus.Free
                });
            }
            await base.OnDisconnected(webSocket);
        }

        public async Task GetUsers(WebSocket webSocket)
        {
            var unionSet = new HashSet<string>();
            foreach (var set in this._userPairLocked.Values)
            {
                unionSet.UnionWith(set);
            }
            var users = await this._appDB.GetAllUsers();
            var usersLock = users.Select(user =>
            {
                user.LockStatus = unionSet.Contains(user.Id.ToString()) ? LockedItem.LockedStatus.Lock : LockedItem.LockedStatus.Free;
                return user;
            });
            var message = new WSMessageToClient(WSMessageToClient.Instractions.AllUsersResponse, usersLock);
            var messageString = JsonConvert.SerializeObject(message);
            await this.SendMessage(webSocket, messageString);
        }

        public async Task GetEditRight(WebSocket webSocket, LockedItem itemToLock)
        {
            var isLocked = false;

            foreach (var pair in this._userPairLocked)
            {
                if (pair.Value.Contains(itemToLock.ItemId.ToString()))
                {
                    isLocked = true;
                    break;
                }
            }

            if (!isLocked)
            {
                this._userPairLocked.TryGetValue(this.ConnectionsManager.GetId(webSocket), out var lockedIdSet);
                lockedIdSet.Add(itemToLock.ItemId.ToString());
            }

            var canUserEdit = !isLocked;
            var message = new WSMessageToClient(WSMessageToClient.Instractions.EditRightResponse, canUserEdit);
            await this.SendMessage(webSocket, JsonConvert.SerializeObject(message));
            if (!isLocked)
            {
                await this.UpdateLock(webSocket, new LockedItem()
                {
                    ItemId = itemToLock.ItemId,
                    Status = LockedItem.LockedStatus.Lock
                });
            }
        }

        public async Task DeleteEditRight(WebSocket webSocket, LockedItem itemToUnlock)
        {
            var id = this.ConnectionsManager.GetId(webSocket);

            this._userPairLocked.TryGetValue(id, out var lockedIdSet);
            lockedIdSet.Remove(itemToUnlock.ItemId.ToString());

            await this.UpdateLock(webSocket, new LockedItem()
            {
                ItemId = itemToUnlock.ItemId,
                Status = LockedItem.LockedStatus.Free
            });
        }

        private async Task UpdateLock(WebSocket webSocket, LockedItem lockedItem)
        {
            var message = new WSMessageToClient(WSMessageToClient.Instractions.UpdateLock, lockedItem);
            await this.SendMessageToAllExcept(webSocket, JsonConvert.SerializeObject(message));
        }
    }
}
