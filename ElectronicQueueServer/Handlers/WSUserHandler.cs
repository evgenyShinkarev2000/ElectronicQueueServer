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
    public class WSUserHandler : SocketHandler
    {
        private readonly AppDB _appDB;
        private readonly ConcurrentDictionary<string, HashSet<string>> _userPairLocked = new ConcurrentDictionary<string, HashSet<string>>();
        public WSUserHandler(ConnectionManager connectionManager, TicketMenager ticketMenager, AppDB appDB)
            : base(connectionManager, ticketMenager)
        {
            _appDB = appDB;
        }

        public override async Task OnConnection(WebSocket socket)
        {
            await base.OnConnection(socket);
            _userPairLocked.TryAdd(ConnectionsManager.GetId(socket), new HashSet<string>());
        }

        public override async Task OnDisconnected(WebSocket webSocket)
        {
            var id = ConnectionsManager.GetId(webSocket);
            _userPairLocked.TryRemove(id, out var lockedIdSet);
            foreach (var lockedId in lockedIdSet)
            {
                await UpdateLock(webSocket, new LockedItem()
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
            foreach (var set in _userPairLocked.Values)
            {
                unionSet.UnionWith(set);
            }
            var users = await _appDB.GetAllUsers();
            var usersLock = users.Select(user =>
            {
                user.LockStatus = unionSet.Contains(user.Id.ToString()) ? LockedItem.LockedStatus.Lock : LockedItem.LockedStatus.Free;
                return user;
            });
            var message = new WSMessageToClient(WSMessageToClient.Instractions.AllUsersResponse, usersLock);
            var messageString = JsonConvert.SerializeObject(message);
            await SendMessage(webSocket, messageString);
        }

        public async Task AddUser(User user)
        {
            await _appDB.AddUser(user);
            await SendMessageToAll(new WSMessageToClient(WSMessageToClient.Instractions.AddUser, user));
        }

        public async Task UpdateUser(User user)
        {
            await _appDB.ReplaceUser(user);
            await SendMessageToAll(new WSMessageToClient(WSMessageToClient.Instractions.UpdateLock, user));
        }

        public async Task DeleteUser(User user)
        {
            await _appDB.DeleteUser(user);
            await SendMessageToAll(new WSMessageToClient(WSMessageToClient.Instractions.DeleteUser, user));
        }

        public async Task GetEditRight(WebSocket webSocket, LockedItem itemToLock)
        {
            var isLocked = false;

            foreach (var pair in _userPairLocked)
            {
                if (pair.Value.Contains(itemToLock.ItemId.ToString()))
                {
                    isLocked = true;
                    break;
                }
            }

            if (!isLocked)
            {
                _userPairLocked.TryGetValue(ConnectionsManager.GetId(webSocket), out var lockedIdSet);
                lockedIdSet.Add(itemToLock.ItemId.ToString());
            }

            var canUserEdit = !isLocked;
            var message = new WSMessageToClient(WSMessageToClient.Instractions.EditRightResponse, canUserEdit);
            await SendMessage(webSocket, JsonConvert.SerializeObject(message));
            if (!isLocked)
            {
                await UpdateLock(webSocket, new LockedItem()
                {
                    ItemId = itemToLock.ItemId,
                    Status = LockedItem.LockedStatus.Lock
                });
            }
        }

        public async Task DeleteEditRight(WebSocket webSocket, LockedItem itemToUnlock)
        {
            var id = ConnectionsManager.GetId(webSocket);

            _userPairLocked.TryGetValue(id, out var lockedIdSet);
            lockedIdSet.Remove(itemToUnlock.ItemId.ToString());

            await UpdateLock(webSocket, new LockedItem()
            {
                ItemId = itemToUnlock.ItemId,
                Status = LockedItem.LockedStatus.Free
            });
        }

        private async Task UpdateLock(WebSocket webSocket, LockedItem lockedItem)
        {
            var message = new WSMessageToClient(WSMessageToClient.Instractions.UpdateLock, lockedItem);
            await SendMessageToAllExcept(webSocket, JsonConvert.SerializeObject(message));
        }
    }
}
