using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers.WSUser
{
    public class AllUser : IWSController
    {
        private readonly AppDB _appDB;
        private readonly SocketHandler socketHandler;
        private readonly WebSocket _webSocket;
        public AllUser(IWSControllerFactory controllerFactory)
        {
            this._appDB = controllerFactory.AppDB;
            this.socketHandler = controllerFactory.SocketHandler;
            this._webSocket = controllerFactory.WebSocket;
        }
        public Task Delete()
        {
            throw new System.NotImplementedException();
        }

        public Task Get()
        {
            //var unionSet = new HashSet<string>();
            //foreach (var set in this.socketHandler.ConnectionsManager_userPairLocked.Values)
            //{
            //    unionSet.UnionWith(set);
            //}
            //var users = await _appDB.GetAllUsers();
            //var usersLock = users.Select(user =>
            //{
            //    user.LockStatus = unionSet.Contains(user.Id.ToString()) ? LockedItem.LockedStatus.Lock : LockedItem.LockedStatus.Free;
            //    return user;
            //});
            //var message = new WSMessageToClient(WSMessageToClient.Instractions.AllUsersResponse, usersLock);
            //var messageString = JsonConvert.SerializeObject(message);
            //await SendMessage(webSocket, messageString);
            throw new System.NotImplementedException();
        }

        public async Task Handle(IEnumerable<string> instructions, object data)
        {
            var handler = new Dictionary<string, System.Func<Task>>()
            {
                {"get", () => this.Get()}
            };

            await handler[instructions.First()]();
        }

        public Task Post()
        {
            throw new System.NotImplementedException();
        }

        public Task Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
