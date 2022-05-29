using ElectronicQueueServer.Models;
using ElectronicQueueServer.SocketsManager;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Handlers.WSUser
{
    public class AllUsers : IWSController
    {
        private readonly AppDB _appDB;
        private readonly SocketHandler socketHandler;
        private readonly WebSocket _webSocket;
        private readonly ILockManeger<WebSocket, string> _lockManeger;
        public AllUsers(IWSControllerFactory controllerFactory)
        {
            this._appDB = controllerFactory.AppDB;
            this.socketHandler = controllerFactory.SocketHandler;
            this._webSocket = controllerFactory.WebSocket;
            this._lockManeger = controllerFactory.LockManeger;
        }
        public Task Delete()
        {
            throw new System.NotImplementedException();
        }

        [RoleValidator(new[] { "ADMIN" })]
        public async Task Get()
        {
            var users = await this._appDB.GetAllUsers();
            var usersLock = users.Select(user =>
            {
                user.LockStatus = this._lockManeger.IsLocked(user.Id.ToString()) 
                ? LockedItem.LockedStatus.Lock 
                : LockedItem.LockedStatus.Free;
                return user;
            });

            await this.socketHandler.SendMessage(_webSocket, new WSMessageToClient(new[] { "post", "allUsers" }, usersLock));
        }

        [RoleValidator(new[] { "ADMIN" })]
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
